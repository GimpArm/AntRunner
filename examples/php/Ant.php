<?php
require('MapPosition.php');

use AntRunner\AntAction as AntAction;
use AntRunner\EchoResponse as EchoResponse;
use AntRunner\GameEvent as GameEvent;
use AntRunner\Item as Item;
use AntRunner\GameState as GameState;
use AntRunner\ItemColor as ItemColor;
use AntRunner\DamageValues as DamageValues;
use AntRunner\ItemBonusValues as ItemBonusValues;

class MyAnt extends AntRunner\Ant
{
	public $Name = "PHP Ant";
	
	private $_mapWidth;
	private $_mapHeight;
	private $_myColor;

	private $_map;
	private $_currentX;
	private $_currentY;
	private $_currentMode;
	private $_lastAction;
	private $_searchPrimary;
	private $_searchSecondary;
		
	public function Initialize($mapWidth, $mapHeight, $antColor, $startX, $startY) {
		$this->_mapWidth = $mapWidth;
		$this->_mapHeight = $mapHeight;
		$this->_myColor = $antColor;
		
		$this->_currentX = $startX;
		$this->_currentY = $startY;
		$this->_currentMode = 0;
		$this->_lastAction = AntAction::Wait;

		//Initialize internal map
		$this->_map = array();
		for ($y=0; $y<$mapHeight; ++$y)
		{
			$this->_map[$y] = array();
			for ($x=0; $x<$mapHeight; ++$x)
			{
				$this->_map[$y][$x] = new MapPosition();
			}
		}
		$this->_map[$this->_currentY][$this->_currentX]->Known = true;
		$this->SetSearchDirection();
	}
	
	public function Tick(GameState $state)
	{
		$this->ProcessEcho($state->Response);
		$this->ProcessGameEvent($state->Event);

		switch ($this->_currentMode)
		{
			//Map mode
			case 0:
				$this->MapMode();
				break;
		}
	}

	private function SetAction($a)
	{
		//Set the action for the current turn.
		$this->Action = $a;
		//Set the last action so we know the direction on the next turn for successful moving and echo response.
		$this->_lastAction = $a;
	}

	private function ProcessEcho(?EchoResponse $response)
	{
		//There was no Echo last turn
		if ($response == null) return;

		//Set map tiles
		switch ($this->_lastAction)
		{
			case AntAction::EchoRight:
				for ($i = 1; $i < $response->Distance; ++$i)
				{
					$this->_map[$this->_currentY][$this->_currentX + $i]->Known = true;
				}

				if ($this->_currentX + $response->Distance < $this->_mapWidth)
				{
					$this->_map[$this->_currentY][$this->_currentX + $response->Distance]->Known = true;
					$this->_map[$this->_currentY][$this->_currentX + $response->Distance]->Item = $response->Item;
				}
				break;
			case AntAction::EchoDown:
				for ($i = 1; $i < $response->Distance; ++$i)
				{
					$this->_map[$this->_currentY + $i][$this->_currentX]->Known = true;
				}

				if ($this->_currentY + $response->Distance < $this->_mapHeight)
				{
					$this->_map[$this->_currentY + $response->Distance][$this->_currentX]->Known = true;
					$this->_map[$this->_currentY + $response->Distance][$this->_currentX]->Item = $response->Item;
				}
				break;
			case AntAction::EchoLeft:
				for ($i = 1; $i < $response->Distance; ++$i)
				{
					$this->_map[$this->_currentY][$this->_currentX - $i]->Known = true;
				}
				if ($this->_currentX - $response->Distance >= 0)
				{
					$this->_map[$this->_currentY][$this->_currentX - $response->Distance]->Known = true;
					$this->_map[$this->_currentY][$this->_currentX - $response->Distance]->Item = $response->Item;
				}
				break;
			case AntAction::EchoUp:
				for ($i = 1; $i < $response->Distance; ++$i)
				{
					$this->_map[$this->_currentY - $i][$this->_currentX]->Known = true;
				}
				if ($this->_currentY - $response->Distance >= 0)
				{
					$this->_map[$this->_currentY - $response->Distance][$this->_currentX]->Known = true;
					$this->_map[$this->_currentY - $response->Distance][$this->_currentX]->Item = $response->Item;
				}
				break;
			default:
				//Should never come here
				return;
		}
	}

	private function CheckFlag($e, $check)
	{
		return $e & $check == $check;
	}

	private function ProcessGameEvent($e)
	{
		if ($this->CheckFlag($e, GameEvent::CollisionDamage))
		{
			//we ran into something
			return;
		}

		//Move was successful, update map position
		switch ($this->_lastAction)
		{
			case AntAction::MoveRight:
				$this->_currentX += 1;
				break;
			case AntAction::MoveDown:
				$this->_currentY += 1;
				break;
			case AntAction::MoveLeft:
				$this->_currentX -= 1;
				break;
			case AntAction::MoveUp:
				$this->_currentY -= 1;
				break;
		}
	}

	private function MapMode()
	{
		//Check left
		if ($this->_currentX > 0 && !$this->_map[$this->_currentY][$this->_currentX - 1]->Known)
		{
			$this->SetAction(AntAction::EchoLeft);
			return;
		}

		//Check Right
		if ($this->_currentX < $this->_mapWidth - 1 && !$this->_map[$this->_currentY][$this->_currentX + 1]->Known)
		{
			$this->SetAction(AntAction::EchoRight);
			return;
		}

		//Check Up
		if ($this->_currentY > 0 && !$this->_map[$this->_currentY - 1][$this->_currentX]->Known)
		{
			$this->SetAction(AntAction::EchoUp);
			return;
		}

		//Check Down
		if ($this->_currentY < $this->_mapHeight - 1 && !$this->_map[$this->_currentY + 1][$this->_currentX]->Known)
		{
			$this->SetAction(AntAction::EchoDown);
			return;
		}

		//All tiles next to us are known, move a direction.
		if ($this->CanMove($this->_searchPrimary))
		{
			$this->SetAction($this->_searchPrimary);
			return;
		}

		if ($this->CanMove($this->_searchSecondary))
		{
			$this->SetAction($this->_searchSecondary);
			return;
		}

		//Can't move in that direction any more, try a different direction.
		if ($this->_lastAction != $this->_searchPrimary && $this->CanMove($this->OppositeDirection($this->_searchPrimary)))
		{
			$this->_searchPrimary = $this->OppositeDirection($this->_searchPrimary);
			$this->SetAction($this->_searchPrimary);
			return;
		}

		if ($this->CanMove($this->OppositeDirection($this->_searchSecondary)))
		{
			$this->_searchSecondary = $this->OppositeDirection($this->_searchSecondary);
			$this->SetAction($this->_searchSecondary);
			return;
		}

		//Can't find a direction, reset current search directions
		$this->SetSearchDirection();
	}

	private function SetSearchDirection()
	{
		if ($this->_currentX == 0 || $this->_mapWidth / $this->_currentX < 0.9)
		{
			//search right
			$this->_searchPrimary = AntAction::MoveRight;
		}
		else
		{
			//search left
			$this->_searchPrimary = AntAction::MoveLeft;
		}

		if ($this->_currentY == 0 || $this->_mapHeight / $this->_currentY < 0.9)
		{
			//search down
			$this->_searchSecondary = AntAction::MoveDown;
		}
		else
		{
			//search up
			$this->_searchSecondary = AntAction::MoveUp;
		}
	}

	private function CanMove($a)
	{
		switch ($a)
		{
			case AntAction::MoveRight:
				if ($this->_currentX + 1 == $this->_mapWidth) return false;
				$nextSpace = $this->_map[$this->_currentY][$this->_currentX + 1];
				break;
			case AntAction::MoveDown:
				if ($this->_currentY + 1 == $this->_mapHeight) return false;
				$nextSpace = $this->_map[$this->_currentY + 1][$this->_currentX];
				break;
			case AntAction::MoveLeft:
				if ($this->_currentX - 1 == -1) return false;
				$nextSpace = $this->_map[$this->_currentY][$this->_currentX - 1];
				break;
			case AntAction::MoveUp:
				if ($this->_currentY - 1 == -1) return false;
				$nextSpace = $this->_map[$this->_currentY - 1][$this->_currentX];
				break;
			default:
				return true;
		}

		return $nextSpace->Known && !$this->Blocked($nextSpace->Item);
	}

	private function Blocked($i)
	{
		switch ($i)
		{
			case Item::SteelWall:
			case Item::BrickWall:
			case Item::Bomb:
				return true;
			case Item::RedHome:
				return $this->_myColor != ItemColor::Red;
			case Item::BlueHome:
				return $this->_myColor != ItemColor::Blue;
			case Item::GreenHome:
				return $this->_myColor != ItemColor::Green;
			case Item::OrangeHome:
				return $this->_myColor != ItemColor::Orange;
			case Item::PinkHome:
				return $this->_myColor != ItemColor::Pink;
			case Item::YellowHome:
				return $this->_myColor != ItemColor::Yellow;
			case Item::GrayHome:
				return $this->_myColor != ItemColor::Gray;
			case Item::WhiteHome:
				return $this->_myColor != ItemColor::White;
			default:
				return false;
		}
	}

	private function OppositeDirection($a)
	{
		switch ($a)
		{
			case AntAction::MoveRight:
				return AntAction::MoveLeft;
			case AntAction::MoveDown:
				return AntAction::MoveUp;
			case AntAction::MoveLeft:
				return AntAction::MoveRight;
			case AntAction::MoveUp:
				return AntAction::MoveDown;
			case AntAction::EchoRight:
				return AntAction::EchoLeft;
			case AntAction::EchoDown:
				return AntAction::EchoUp;
			case AntAction::EchoLeft:
				return AntAction::EchoRight;
			case AntAction::EchoUp:
				return AntAction::EchoDown;
			case AntAction::ShootRight:
				return AntAction::ShootLeft;
			case AntAction::ShootDown:
				return AntAction::ShootUp;
			case AntAction::ShootLeft:
				return AntAction::ShootRight;
			case AntAction::ShootUp:
				return AntAction::ShootDown;
			default:
				return AntAction::Wait;
		}
	}
}
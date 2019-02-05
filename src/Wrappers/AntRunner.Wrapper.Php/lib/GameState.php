<?php namespace AntRunner;

class GameState
{
	public $TickNumber;
	public $HasFlag;
	public $FlagX;
	public $FlagY;
	public $AntWithFlag;
	public $Response;
	public $Event;
		
	function __construct($tickNumber, $event = GameEvent::Nothing, $response = null, $flagX = -1, $flagY = -1, $antWithFlag = ItemColor::None)
	{
		$this->TickNumber = $tickNumber;
		$this->HasFlag = $flagX != -1;
		$this->FlagX = $flagX;
		$this->FlagY = $flagY;
		$this->AntWithFlag = $antWithFlag;
		$this->Response = $response;
		$this->Event = $event;
	}
}
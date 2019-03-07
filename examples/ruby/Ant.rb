include AntRunner::Interface

require_relative 'MapPosition.rb'

class MyAnt < AntRunner::Interface::Ant
	def Name
		"Ruby Ant"
	end

	def Initialize(mapWidth, mapHeight, color, startX, startY)
		@MapWidth = mapWidth
		@MapHeight = mapHeight
		@MyColor = color
		@CurrentX = startX
		@CurrentY = startY

		@CurrentMode = 0
		@LastAction = AntAction::Wait

		@Map = Array.new(mapHeight) { Array.new(mapWidth)}
		y = 0
		until y >= mapHeight
			x = 0
			until x >= mapWidth
				@Map[y][x] = MapPosition.new
				x += 1
			end
			y += 1
		end
		
		@Map[startY][startX].Known = true		
		SetSearchDirection()
	end
		
	def Tick(state)
		ProcessEcho(state.Response)
		ProcessGameEvent(state.Event)

		if @CurrentMode == 0
			MapMode()
		end
	end
	
	def SetAction(a)
		@Action = a
		@LastAction = a
	end
	
	def ProcessEcho(response)
		if response.nil?
			return
		end
		
		case @LastAction
		when AntAction::EchoRight
			i = 1
			until i >= response.Distance
				@Map[@CurrentY][@CurrentX + i].Known = true
				i += 1
			end

			if @CurrentX + response.Distance < @MapWidth
				@Map[@CurrentY][@CurrentX + response.Distance].Known = true
				@Map[@CurrentY][@CurrentX + response.Distance].Item = response.Item
			end
		when AntAction::EchoDown
			i = 1
			until i >= response.Distance
				@Map[@CurrentY + i][@CurrentX ].Known = true
				i += 1
			end

			if @CurrentY + response.Distance < @MapHeight
				@Map[@CurrentY + response.Distance][@CurrentX].Known = true
				@Map[@CurrentY + response.Distance][@CurrentX].Item = response.Item
			end
		when AntAction::EchoLeft
			i = 1
			until i >= response.Distance
				@Map[@CurrentY][@CurrentX - i].Known = true
				i += 1
			end

			if @CurrentX - response.Distance >= 0
				@Map[@CurrentY][@CurrentX - response.Distance].Known = true
				@Map[@CurrentY][@CurrentX - response.Distance].Item = response.Item
			end
		when AntAction::EchoUp
			i = 1
			until i >= response.Distance
				@Map[@CurrentY - i][@CurrentX ].Known = true
				i += 1
			end

			if @CurrentY - response.Distance >= 0
				@Map[@CurrentY - response.Distance][@CurrentX].Known = true
				@Map[@CurrentY - response.Distance][@CurrentX].Item = response.Item
			end
		else
			#Should never come here
		end
	end

	def CheckFlag(e, check)
		return e & check == check
	end

	def ProcessGameEvent(e)
		if CheckFlag(e, GameEvent::CollisionDamage)
			return
		end
		
		case @LastAction
		when AntAction::MoveRight
				@CurrentX += 1
		when AntAction::MoveDown
				@CurrentY += 1
		when AntAction::MoveLeft
				@CurrentX -= 1
		when AntAction::MoveUp
				@CurrentY -= 1
		end
	end

	def MapMode()
		#Check left
		if @CurrentX > 0 && !@Map[@CurrentY][@CurrentX - 1].Known
			SetAction(AntAction::EchoLeft)
			return
		end

		#Check right
		if @CurrentX < @MapWidth - 1 && !@Map[@CurrentY][@CurrentX + 1].Known
			SetAction(AntAction::EchoRight)
			return
		end

		#Check up
		if @CurrentY > 0 && !@Map[@CurrentY - 1][@CurrentX].Known
			SetAction(AntAction::EchoUp)
			return
		end

		#Check  down
		if @CurrentY < @MapHeight - 1 && !@Map[@CurrentY + 1][@CurrentX].Known
			SetAction(AntAction::EchoDown)
			return
		end

		#All tiles next to us are known, move a direction.
		if CanMove(@SearchPrimary)
			SetAction(@SearchPrimary)
			return
		end

		if CanMove(@SearchSecondary)
			SetAction(@SearchSecondary)
			return
		end

		if @LastAction != @SearchPrimary && CanMove(OppositeDirection(@SearchPrimary))
			@SearchPrimary = OppositeDirection(@SearchPrimary)
			SetAction(@SearchPrimary)
			return
		end

		if CanMove(OppositeDirection(@SearchSecondary))
			@SearchSecondary = OppositeDirection(@SearchSecondary)
			SetAction(@SearchSecondary)
			return
		end
		
		SetSearchDirection()
	end

	def SetSearchDirection()
		if @CurrentX == 0 or @MapWidth / @CurrentX < 0.9
			@SearchPrimary = AntAction::MoveRight
		else
			@SearchPrimary = AntAction::MoveLeft
		end

		if @CurrentY == 0 or @MapHeight / @CurrentY < 0.9
			@SearchSecondary = AntAction::MoveDown
		else
			@SearchSecondary = AntAction::MoveUp
		end
	end
		
	def CanMove(a)
		nextSpace = nil
		case a 
		when AntAction::MoveRight
			if @CurrentX + 1 == @MapWidth
				return false
			end
			nextSpace = @Map[@CurrentY][@CurrentX + 1]
		when AntAction::MoveDown
			if @CurrentY + 1 == @MapHeight
				return false
			end
			nextSpace = @Map[@CurrentY + 1][@CurrentX]
		when AntAction::MoveLeft
			if @CurrentX - 1 == -1
				return false
			end
			nextSpace = @Map[@CurrentY][@CurrentX - 1]
		when AntAction::MoveUp
			if @CurrentY - 1 == -1
				return false
			end
			nextSpace = @Map[@CurrentY - 1][@CurrentX]
		else
			return true
		end

		return nextSpace.Known && !Blocked(nextSpace.Item)
	end
	
	def Blocked(i)
		case i
		when Item::SteelWall, Item::BrickWall, Item::Bomb
			return true		
		when Item::RedHome
			return @MyColor != ItemColor::Red
		when Item::BlueHome
			return @MyColor != ItemColor::Blue
		when Item::GreenHome
			return @MyColor != ItemColor::Green
		when Item::OrangeHome
			return @MyColor != ItemColor::Orange
		when Item::PinkHome
			return @MyColor != ItemColor::Pink
		when Item::YellowHome
			return @MyColor != ItemColor::Yellow
		when Item::GrayHome
			return @MyColor != ItemColor::Gray
		when Item::WhiteHome
			return @MyColor != ItemColor::White
		else
			return false
		end
	end

	def OppositeDirection(a)
		case a
		when AntAction::MoveRight
			return AntAction::MoveLeft
		when AntAction::MoveDown
			return AntAction::MoveUp
		when AntAction::MoveLeft
			return AntAction::MoveRight
		when AntAction::MoveUp
			return AntAction::MoveDown
		when AntAction::EchoRight
			return AntAction::EchoLeft
		when AntAction::EchoDown
			return AntAction::EchoUp
		when AntAction::EchoLeft
			return AntAction::EchoRight
		when AntAction::EchoUp
			return AntAction::EchoDown			
		when AntAction::ShootRight
			return AntAction::ShootLeft
		when AntAction::ShootDown
			return AntAction::ShootUp
		when AntAction::ShootLeft
			return AntAction::ShootRight
		when AntAction::ShootUp
			return AntAction::ShootDown
		else
			return AntAction::Wait
		end
	end
end
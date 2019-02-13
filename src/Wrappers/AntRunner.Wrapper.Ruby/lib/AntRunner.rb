require "json"

module AntRunner
	module Interface
		class Ant			
			def Name
				"No Name"
			end

			def Action
				@Action
			end

			def Action= v
				@Action = v
			end
			
			def Initialize(mapWidth, mapHeight, color, startX, startY)
				#Initialize code
			end

			def Tick(state)
				#Tick code
			end

			def self.Descendants()
				ObjectSpace.each_object(::Class).select {|klass| klass < self }
			end
		end

		module AntAction
			Wait = 0
			MoveRight = 1
			MoveDown = 2
			MoveLeft = 3
			MoveUp = 4
			EchoRight = 5
			EchoDown = 6
			EchoLeft = 7
			EchoUp = 8
			ShieldOn = 9
			ShieldOff = 10
			DropBomb = 11
			ShootRight = 12
			ShootDown = 13
			ShootLeft = 14
			ShootUp = 15
		end

		module Item
			Empty = 0
			SteelWall = 1
			BrickWall = 2
			Bomb = 3
			PowerUpBomb = 4
			PowerUpHealth = 5
			PowerUpShield = 6
			RedAnt = 7
			BlueAnt = 8
			GreenAnt = 9
			OrangeAnt = 10
			PinkAnt = 11
			YellowAnt = 12
			GrayAnt = 13
			WhiteAnt = 14
			RedHome = 15
			BlueHome = 16
			GreenHome = 17
			OrangeHome = 18
			PinkHome = 19
			YellowHome = 20
			GrayHome = 21
			WhiteHome = 22
			Flag = 23
		end

		module GameEvent
			Nothing = 0
			CollisionDamage = 1
			ImpactDamageRight = 2
			ImpactDamageDown = 4
			ImpactDamageLeft = 8
			ImpactDamageUp = 16
			ShotDamageRight = 32
			ShotDamageDown = 64
			ShotDamageLeft = 128
			ShotDamageUp = 256
			BombDamage = 512
			PickUpBomb = 1024
			PickUpShield = 2048
			PickUpHealth = 4096
			PickUpFlag = 8192
			Dead = 16384
			GameOver = 32768
		end

		module ItemColor
			None = 0
			Red = 1
			Blue = 2
			Green = 3
			Orange = 4
			Pink = 5
			Yellow = 6
			Gray = 7
			White = 8
		end

		module ItemBonusValues
			Health = 25
			Shield = 25
			Bomb = 4
		end

		module DamageValues
			Collision = 5
			Impact = 10
			Shot = 20
			Bomb = 30
		end

		class EchoResponse
			attr_accessor :Distance, :Item

			def initialize(distance, item)
				@Distance = distance
				@Item = item
			end
			
			def self.from_json(string)
				data = JSON.load(string)
				return self.new data['Distance'], data['Item']
			end
		end

		class GameState
			attr_accessor :TickNumber, :HasFlag, :FlagX, :FlagY, :AntWithFlag, :Response, :Event

			def initialize(tickNumber, event = GameEvent::Nothing, response = nil, flagX = -1, flagY = -1, antWithFlag = ItemColor::None)
				@TickNumber = tickNumber
				@Event = event
				@Response = response
				@HasFlag = flagX != -1
				@FlagX = flagX
				@FlagY = flagY
				@AntWithFlag = antWithFlag
			end

			def self.from_json(string)
				data = JSON.load(string)
				return self.new(data['TickNumber'], data['Event'], data['Response'].nil? ? nil : EchoResponse.new(data['Response']['Distance'], data['Response']['Item']), data['FlagX'], data['FlagY'], data['AntWithFlag'])
			end
		end		
	end
end
import clr
clr.AddReference("AntRunner.Interface")
import AntRunner.Interface.Ant

from AntRunner.Interface import *

from MapPosition import MapPosition

class MyAnt(AntRunner.Interface.Ant):
	Name = "Python Ant"

	MapWidth = 0
	MapHeight = 0

	CurrentX = 0
	CurrentY = 0

	MyColor = ItemColor.None

	Map = []

	CurrentMode = 0
	LastAction = AntAction.Wait
	SearchPrimary = 0
	SearchSecondary = 0
	
	def Initialize(self, mapWidth, mapHeight, color, startX, startY):
		self.MapWidth = mapWidth
		self.MapHeight = mapHeight
		self.MyColor = color
		self.CurrentX = startX
		self.CurrentY = startY

		self.CurrentMode = 0
		self.LastAction = AntAction.Wait

		self.Map = [[0 for x in xrange(mapWidth)] for y in xrange(mapHeight)] 
		for y in xrange(0, mapHeight):
			self.Map.append(y)
			for x in xrange(0, mapWidth):
				self.Map[y][x] = MapPosition()
		
		self.Map[startY][startX].Known = True		
		self.SetSearchDirection()
		
		
	def Tick(self, state):
		self.ProcessEcho(state.Response)
		self.ProcessGameEvent(state.Event)

		if self.CurrentMode == 0:
			self.MapMode()
	
	def SetAction(self, a):
		self.Action = a
		self.LastAction = a
	
	def ProcessEcho(self, response):
		if response == None:
			return
		
		if self.LastAction == AntAction.EchoRight:
			for i in xrange(1, response.Distance):
				self.Map[self.CurrentY][self.CurrentX + i].Known = True

			if self.CurrentX + response.Distance < self.MapWidth:
				self.Map[self.CurrentY][self.CurrentX + response.Distance].Known = True
				self.Map[self.CurrentY][self.CurrentX + response.Distance].Item = response.Item
		elif self.LastAction == AntAction.EchoDown:
			for i in xrange(1, response.Distance):
				self.Map[self.CurrentY + i][self.CurrentX ].Known = True

			if self.CurrentY + response.Distance < self.MapWidth:
				self.Map[self.CurrentY + response.Distance][self.CurrentX].Known = True
				self.Map[self.CurrentY + response.Distance][self.CurrentX].Item = response.Item
		elif self.LastAction == AntAction.EchoLeft:
			for i in xrange(1, response.Distance):
				self.Map[self.CurrentY][self.CurrentX - i].Known = True

			if self.CurrentX - response.Distance >= 0:
				self.Map[self.CurrentY][self.CurrentX - response.Distance].Known = True
				self.Map[self.CurrentY][self.CurrentX - response.Distance].Item = response.Item
		elif self.LastAction == AntAction.EchoUp:
			for i in xrange(1, response.Distance):
				self.Map[self.CurrentY - i][self.CurrentX ].Known = True

			if self.CurrentY - response.Distance >= 0:
				self.Map[self.CurrentY - response.Distance][self.CurrentX].Known = True
				self.Map[self.CurrentY - response.Distance][self.CurrentX].Item = response.Item
		else:
			pass #Should never come here

	def CheckFlag(self, e, check):
		return e & check == check

	def ProcessGameEvent(self, e):
		if self.CheckFlag(e, GameEvent.CollisionDamage):
			return
		
		if self.LastAction == AntAction.MoveRight:
				self.CurrentX += 1
		elif self.LastAction == AntAction.MoveDown:
				self.CurrentY += 1
		elif self.LastAction == AntAction.MoveLeft:
				self.CurrentX -= 1
		elif self.LastAction == AntAction.MoveUp:
				self.CurrentY -= 1

	def MapMode(self):
		#Check left
		if self.CurrentX > 0 and not self.Map[self.CurrentY][self.CurrentX - 1].Known:
			self.SetAction(AntAction.EchoLeft)
			return

		#Check right
		if self.CurrentX < self.MapWidth - 1 and not self.Map[self.CurrentY][self.CurrentX + 1].Known:
			self.SetAction(AntAction.EchoRight)
			return

		#Check up
		if self.CurrentY > 0 and not self.Map[self.CurrentY - 1][self.CurrentX].Known:
			self.SetAction(AntAction.EchoUp)
			return

		#Check  down
		if self.CurrentY < self.MapHeight - 1 and not self.Map[self.CurrentY + 1][self.CurrentX].Known:
			self.SetAction(AntAction.EchoDown)
			return

		#All tiles next to us are known, move a direction.
		if self.CanMove(self.SearchPrimary):
			self.SetAction(self.SearchPrimary)
			return

		if self.CanMove(self.SearchSecondary):
			self.SetAction(self.SearchSecondary)
			return

		if self.LastAction != self.SearchPrimary and self.CanMove(self.OppositeDirection(self.SearchPrimary)):
			self.SearchPrimary = self.OppositeDirection(self.SearchPrimary)
			self.SetAction(self.SearchPrimary)
			return

		if self.CanMove(self.OppositeDirection(self.SearchSecondary)):
			self.SearchSecondary = self.OppositeDirection(self.SearchSecondary)
			self.SetAction(self.SearchSecondary)
			return
		
		self.SetSearchDirection()

	def SetSearchDirection(self):
		if self.CurrentX == 0 or self.MapWidth / self.CurrentX < 0.9:
			self.SearchPrimary = AntAction.MoveRight
		else:
			self.SearchPrimary = AntAction.MoveLeft

		if self.CurrentY == 0 or self.MapHeight / self.CurrentY < 0.9:
			self.SearchSecondary = AntAction.MoveDown
		else:
			self.SearchSecondary = AntAction.MoveUp
		
	def CanMove(self, a):
		nextSpace = None
		if a == AntAction.MoveRight:
			if self.CurrentX + 1 == self.MapWidth:
				return False
			nextSpace = self.Map[self.CurrentY][self.CurrentX + 1]
		elif a == AntAction.MoveDown:
			if self.CurrentY + 1 == self.MapHeight:
				return False
			nextSpace = self.Map[self.CurrentY + 1][self.CurrentX]
		elif a == AntAction.MoveLeft:
			if self.CurrentX - 1 == -1:
				return False
			nextSpace = self.Map[self.CurrentY][self.CurrentX - 1]
		elif a == AntAction.MoveUp:
			if self.CurrentY - 1 == -1:
				return False
			nextSpace = self.Map[self.CurrentY - 1][self.CurrentX]
		else:
			return True

		return nextSpace.Known and not self.Blocked(nextSpace.Item)
	
	def Blocked(self, i):
		if i == Item.SteelWall or i == Item.BrickWall or i == Item.Bomb:
			return True
		if i == Item.RedHome:
			return self.MyColor != ItemColor.Red
		if i == Item.BlueHome:
			return self.MyColor != ItemColor.Blue
		if i == Item.GreenHome:
			return self.MyColor != ItemColor.Green
		if i == Item.OrangeHome:
			return self.MyColor != ItemColor.Orange
		if i == Item.PinkHome:
			return self.MyColor != ItemColor.Pink
		if i == Item.YellowHome:
			return self.MyColor != ItemColor.Yellow
		if i == Item.GrayHome:
			return self.MyColor != ItemColor.Gray
		if i == Item.WhiteHome:
			return self.MyColor != ItemColor.White

		return False

	def OppositeDirection(self, a):
		if a == AntAction.MoveRight:
			return AntAction.MoveLeft
		if a == AntAction.MoveDown:
			return AntAction.MoveUp
		if a == AntAction.MoveLeft:
			return AntAction.MoveRight
		if a == AntAction.MoveUp:
			return AntAction.MoveDown	

		if a == AntAction.EchoRight:
			return AntAction.EchoLeft
		if a == AntAction.EchoDown:
			return AntAction.EchoUp
		if a == AntAction.EchoLeft:
			return AntAction.EchoRight
		if a == AntAction.EchoUp:
			return AntAction.EchoDown
			
		if a == AntAction.ShootRight:
			return AntAction.ShootLeft
		if a == AntAction.ShootDown:
			return AntAction.ShootUp
		if a == AntAction.ShootLeft:
			return AntAction.ShootRight
		if a == AntAction.ShootUp:
			return AntAction.ShootDown

		return AntAction.Wait
<?php namespace AntRunner;

abstract class AntAction
{
    const Wait = 0;
    const MoveRight = 1;
	const MoveDown = 2;
	const MoveLeft = 3;
	const MoveUp = 4;
	const EchoRight = 5;
	const EchoDown = 6;
	const EchoLeft = 7;
	const EchoUp = 8;
	const ShieldOn = 9;
	const ShieldOff = 10;
	const DropBomb = 11;
	const ShootRight = 12;
	const ShootDown = 13;
	const ShootLeft = 14;
	const ShootUp = 15;
}
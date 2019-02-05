<?php namespace AntRunner;

abstract class GameEvent
{
    const Nothing = 0x00000;
    const CollisionDamage = 0x00001;
	const ImpactDamageRight = 0x00002;
	const ImpactDamageDown = 0x00004;
	const ImpactDamageLeft = 0x00008;
	const ImpactDamageUp = 0x00010;
	const ShotDamageRight = 0x00020;
	const ShotDamageDown = 0x00040;
	const ShotDamageLeft = 0x00080;
	const ShotDamageUp = 0x00100;
	const BombDamage = 0x000200;
	const PickUpBomb = 0x00400;
	const PickUpShield = 0x00800;
	const PickUpHealth = 0x01000;
	const PickUpFlag = 0x02000;
	const Dead = 0x04000;
	const GameOver = 0x08000;
}
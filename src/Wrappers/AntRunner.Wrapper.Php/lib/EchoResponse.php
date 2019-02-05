<?php namespace AntRunner;

class EchoResponse
{
	public $Distance;
	public $Item;

	function __construct($distance, $item)
	{
		$this->Distance = $distance;
		$this->Item = $item;
	}
}
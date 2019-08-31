<?php
if (!file_exists($argv[1])) throw new Exception('Ant file "' . $argv[1] . '" does not exist!');

require(__DIR__ . '/AntRunner.php');

Run($argv[1]);

function Run($filename)
{
	$ant = LoadClass($filename);
	if ($ant == null) throw new Exception('Count not find valid Ant class in "' . $filename . '"!');
	
	$stdin = fopen('php://stdin', 'r');
	while (FALSE !== ($line = trim(fgets($stdin))))
	{
		try
		{
			if (empty($line)) continue;
			switch (substr($line, 0, 1))
			{
				case 'T':
					$ant->Tick(unserialize(substr($line, 1)));
					fwrite(STDOUT, "ok" . PHP_EOL);
					break;
				case 'A':
					$a = (int)$ant->Action;
					$ant->Action = 0;
					fwrite(STDOUT, $a . PHP_EOL);
					break;
				case 'I':
					$init = unserialize(substr($line, 1));
					$ant->Initialize($init[0], $init[1], $init[2], $init[3], $init[4]);
					break;
				case 'N':
					fwrite(STDOUT, $ant->Name . PHP_EOL);
					break;
				case 'P':
					fwrite(STDOUT, 'Ping' . PHP_EOL);
					break;
				case 'X':
					break 2;
				default:
					fwrite(STDOUT, 'Invalid command' . PHP_EOL);
					break;
			}
		} catch (Exception $e) {
			
		}
		fflush(STDOUT);
	}

	fclose($stdin);
}

function LoadClass($filename)
{
	$classes = get_declared_classes();
	
	require_once($filename);
	$diff = array_diff(get_declared_classes(), $classes);
	
	foreach ($diff as $className)
	{
		$obj = new ReflectionClass($className);
		if ($obj->isSubclassOf('\AntRunner_Interface\Ant'))
		{
			return new $className();
		}
	}
	return null;
}
new SimGroup("SM_Command_Queue")
{
	tasksQueued = 0;
};

function SM_Command_Queue::addToQueue(%this,%time,%command,%a,%checkA,%b,%checkB,%c,%checkC,%d,%checkD,%e,%checkE)
{
	%taskCount = %this.tasksQueued + 1;

	%task =	new ScriptObject("Task" @ %taskCount)
	{
		time = %time;
		command = %command;
		argA = %a;
		argACheck = %checkA;
		argB = %b;
		argBCheck = %checkB;
		argC = %c;
		argCCheck = %checkC;
		argD = %d;
		argDCheck = %checkD;
		argE = %e;
		argECheck = %checkE;
	};
	
	%this.add(%task);
}

function SM_Command_Queue::removeFromQueue(%this,%task)
{
	if(%this.getCount() != %this.tasksQueued)
		return error("ERROR: Task malfunction. The task counts are not equal.");
		
	for(%i = 0; %i < %this.getCount(); %i++)
	{
		%obj = %this.getObject(%i);
		if(%obj.getName() $= %task)
		{
			%obj.delete();
			%this.tasksQueued -= 1;
		}
	}
}

function SM_Command_Queue::startTask(%this)
{
}
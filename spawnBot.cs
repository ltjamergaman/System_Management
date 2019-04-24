//$botspawn exists
function spawnBots(%amount, %class)
{
   //Does it %amount amount of times.
   for(%i=0;%i<%amount;%i++)
   {
      //Creates the neccisary spawn brick. Otherwise bot kablooey-s.
      if(!isObject(%brick = nameToID(UselessRoboBrick)))
      {
         %brick = new fxDtsBrick(UselessRoboBrick)
         {
            datablock = brick1x1Data;
            isPlanted = false;
            itemPosition = 1;
            position = $botSpawn;
         };
      }
      schedule(33, 0, createRobot);
   }
}

function createRobot()
{
   talk("aklamo");
   //Creates the bot.
   %robot = new AiPlayer()
   {
      spawnTime = $Sim::Time;
      spawnBrick = nameToID(UselessRoboBrick);
      dataBlock = "ZombieHoleBot";
      position = $botSpawn;

      //Springtime for Hitler
      Name = ZombieHoleBot.hName;
      hType = ZombieHoleBot.hType;
      hSearchRadius = ZombieHoleBot.hSearchRadius;
      hSearch = ZombieHoleBot.hSearch;
      hSight = ZombieHoleBot.hSight;
      hWander = ZombieHoleBot.hWander;
      hGridWander = ZombieHoleBot.hGridWander;
      hReturnToSpawn = ZombieHoleBot.hReturnToSpawn;
      hSpawnDist = ZombieHoleBot.hSpawnDist;
      hMelee = ZombieHoleBot.hMelee;
      hAttackDamage = ZombieHoleBot.hAttackDamage;
      hSpazJump = ZombieHoleBot.hSpazJump;
      hSearchFOV = ZombieHoleBot.hSearchFOV;
      hFOVRadius = ZombieHoleBot.hFOVRadius;
      hTooCloseRange = ZombieHoleBot.hTooCloseRange;
      hAvoidCloseRange = ZombieHoleBot.hAvoidCloseRange;
      hShoot = ZombieHoleBot.hShoot;
      hMaxShootRange = ZombieHoleBot.hMaxShootRange;
      hStrafe = ZombieHoleBot.hStrafe;
      hAlertOtherBots = ZombieHoleBot.hAlertOtherBots;
      hIdleAnimation = ZombieHoleBot.hIdleAnimation;
      hSpasticLook = ZombieHoleBot.hSpasticLook;
      hAvoidObstacles = ZombieHoleBot.hAvoidObstacles;
      hIdleLookAtOthers = ZombieHoleBot.hIdleLookAtOthers;
      hIdleSpam = ZombieHoleBot.hIdleSpam;
      hAFKOmeter = ZombieHoleBot.hAFKOmeter + getRandom( 0, 2 );
      hHearing = ZombieHoleBot.hHearing;
      hIdle = ZombieHoleBot.hIdle;
      hSmoothWander = ZombieHoleBot.hSmoothWander;
      hEmote = ZombieHoleBot.hEmote;
      hSuperStacker = ZombieHoleBot.hSuperStacker;
      hNeutralAttackChance = ZombieHoleBot.hNeutralAttackChance;
      hFOVRange = ZombieHoleBot.hFOVRange;
      hMoveSlowdown = ZombieHoleBot.hMoveSlowdown;
      hMaxMoveSpeed = 1.0;
      hActivateDirection = ZombieHoleBot.hActivateDirection;
      isHoleBot = 1;
   };
   missionCleanup.add(%robot);
}

//Experimental stuff, trying to fix the problem.
package xXhacksXx2
{
   function hBrickClientCheck(%brickgroup)
   {
      talk("cl check");
      return 1;
   }
};
activatePackage(xXhacksXx2);
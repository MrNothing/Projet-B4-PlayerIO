/*
	Generated with B4 Built-in Editor
		999* tiles
		999* pathTiles
		999* events
		999* entities
*/

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace PhotonB4
{
	public class MapsData
	{
		public void loadMap(GameCode core, String map)
        {
            if (map.Equals("Map1"))
            {
                Map map_0 = new Map("Fields");
                map_0.name = "Map1";

                UnitsInfos tmpInfos = new UnitsInfos();

                string[] mobsGrp1 = new string[1];
                mobsGrp1[0] = "Blob";

                SpawnZone pack1 = new SpawnZone(core, new Vector3(-0.1796811f, 2.732747f, -4.319699f), mobsGrp1, 20);
                pack1.maxAmountSimulaneously = 10;

                core.spawnZones.Add("blobs1", pack1);

                string[] mobsskel = new string[1];
                mobsskel[0] = "Skeleton";

                SpawnZone pack2 = new SpawnZone(core, new Vector3(-40, 5.400879f, -4.319699f), mobsskel, 20);
                pack2.maxAmountSimulaneously = 10;

                core.spawnZones.Add("skel", pack2);

                Entity testEnt1 = new Entity(core, "", "Ent", tmpInfos.getEntityInfosByName("Ent"), new Vector3(0, 2.732747f, -30));
                testEnt1.team = "";
                core.addUnit(testEnt1);

                Entity testDrake1 = new Entity(core, "", "Black Drake", tmpInfos.getEntityInfosByName("BlackDrake"), new Vector3(0, 35f, 0));
                testDrake1.team = "";
                testDrake1.wanderAround = new Vector3(200, 0, 200);
                core.addUnit(testDrake1);

                Entity testGuard = new Entity(core, "", "Guard", tmpInfos.getEntityInfosByName("Guard"), new Vector3(-10.00516f, 2.918638f, 89.88609f));
                core.addUnit(testGuard);
            }
        }
	}
}
﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using PlayerIO.GameLibrary;

namespace PhotonB4
{
    public class GameManager
    {
        GameCode mainInstance;
        SpellsManager spellsInstance;
        public GameManager(GameCode _mainInstance, SpellsManager _spellsInstance)
        {
            spellsInstance = _spellsInstance;
            mainInstance = _mainInstance;
        }

        public void handleClientRequest(Player sender,string _cmd, Message message) 
	    {
            try
            {
                if (sender.myCharacter.locked)
                    return;
            }
            catch (Exception e)
            {
                mainInstance.PlayerIO.ErrorLog.WriteError("handleClientRequest Cancelled: " + _cmd + " there was no Hero loaded");
            }

                //generateItem
                if (_cmd.Equals("startQuest"))
                {
                    ;
                }

                //generateItem
                if (_cmd.Equals("generateItem"))
                {
                    sender.myCharacter.addItem(mainInstance.itemGenerator.generateItem("", 10, 10));
                }
                
                //getVisibleUnits
                if (_cmd.Equals("getVisibleUnits"))
                {
                    try
                    {
                        mainInstance.units[message.GetString(1)].watcher = sender.myCharacter.id;
                    }
                    catch (Exception e)
                    {
                        sender.Send("err", "Unit not found!");
                    }
                }
                
                //getVisibleUnits
                if (_cmd.Equals("ul"))
                {
                    Entity myPlayer = sender.myCharacter;

                    List<String> pList = new List<String>();
                    foreach (string s in mainInstance.units.Keys)
                    {
                        if(myPlayer.get2DDistance(mainInstance.units[s].position)<mainInstance.baseRefSize && mainInstance.units[s].movementCounter>0)
                            pList.Add(s + "|x" + mainInstance.units[s].position.x + "y" + mainInstance.units[s].position.y + "z" + mainInstance.units[s].position.z);
                    }

                    sender.Send("ul", pList.ToArray());
                }
                
                //lvlUpSpell
                if (_cmd.Equals("clearSpells"))
                {
                    Player myPlayer = sender;

                    myPlayer.myCharacter.spells = new Dictionary<string, System.Collections.Hashtable>();
                    myPlayer.myCharacter.spellsByName = new System.Collections.Hashtable();
                    myPlayer.myCharacter.sendSpells();
                }
                
                if (_cmd.Equals("clearItems"))
                {
                    Player myPlayer = sender;

                    myPlayer.myCharacter.items = new Dictionary<string,Item>();
                    myPlayer.myCharacter.itemsByName = new Dictionary<string,float>();
                    myPlayer.myCharacter.equippedItems = new Dictionary<string,string>();
                    myPlayer.myCharacter.sendItems(myPlayer);
                }
                
                //lvlUpSpell
                if (_cmd.Equals("lvlUpSpell"))
                {
                    Player myPlayer = sender;

                    myPlayer.myCharacter.levelUpSpell(message.GetString(1));   
                }

                //lvlUpSpell
                if (_cmd.Equals("teleport"))
                {
                    sender.myCharacter.position = new Vector3(message.GetFloat(1), message.GetFloat(2), message.GetFloat(3));
                    sender.myCharacter.locked = true;
                }


                if (_cmd.Equals("addSpell"))
                {
                    //TODO: do this in a way specific to my class or with a spell Master
                    Player myPlayer = sender;

                   
                    //if (myPlayer.myCharacter.spells.Count < 10)
                        myPlayer.myCharacter.addSpell(message.GetString(1));
                    //else
                    //    sender.Send("err", "You cannot learn any more spells");
                      
                }

                if (_cmd.Equals("spells"))
                {
                    //TODO: do this in a way specific to my class or with a spell Master
                    sender.myCharacter.sendSpells(sender);
                }

                if (_cmd.Equals("items"))
                {
                    //TODO: do this in a way specific to my class or with a spell Master
                    sender.myCharacter.sendItems(sender);
                }

                if (_cmd.Equals("pickUp"))
                {
                    //TODO: do this in a way specific to my class or with a spell Master
                    //message.GetString(1) the mob checked
                    //message.GetInt(2) the item picked
                    Item myItem = sender.myCharacter.itemRewardsByEntity[message.GetString(1)][message.GetInt(2)];

                    sender.myCharacter.itemRewardsByEntity[message.GetString(1)] = null;

                    sender.myCharacter.addItem(myItem);
                }

                if (_cmd.Equals("pickGold"))
                {
                    //TODO: do this in a way specific to my class or with a spell Master
                    //message.GetString(1) the mob checked
                    sender.money += sender.myCharacter.goldRewardsByEntity[message.GetString(1)];

                    sender.myCharacter.goldRewardsByEntity[message.GetString(1)] = 0;
                    
                    sender.myCharacter.sendMoney("");
                }

                if (_cmd.Equals("getRewards"))
                {
                   
                    //message.GetString(1) the mob checked
                    Hashtable items = new Hashtable();

                    //string test = "";

                    for (int i = 0; i < sender.myCharacter.itemRewardsByEntity[message.GetString(1)].Count; i++)
                    {
                        Item myItem = sender.myCharacter.itemRewardsByEntity[message.GetString(1)][i];

                        if (myItem != null)
                        {
                            myItem.infos.description = myItem.generateDescription();
                            items.Add(i + "", myItem.infos.toReducedHashtable());
                        }
                        else
                            items.Add(i + "", "null");
                    }

                    string itemsData = mainInstance.itemGenerator.hashMapToData(items);

                    int money = sender.myCharacter.goldRewardsByEntity[message.GetString(1)];

                    Object[] data = new Object[3];
                    data[0] = money; //i
                    data[1] = itemsData;  //x
                    data[2] = message.GetString(1);  //x

                    sender.Send("rewards", data);
                }

                /*if(_cmd.Equals("profile"))
                {
                    if(FlashFighters.getPlayerByName(params.getUtfString("name"))!=null)
                    {
                        Player target = FlashFighters.getPlayerByName(params.getUtfString("name"));
				
                        if(target.hero!=null)
                        {
                            ISFSObject infos = new SFSObject();
                            infos.putUtfString("id", target.hero.id);
                            infos.putSFSObject("items", target.hero.equippedItems);
                            infos.putSFSObject("stats", target.hero.infos.toSFSObject());
                            send("profile", infos, sender);
                        }
                        else
                        {
                            ISFSObject infos = new SFSObject();
                            infos.putUtfString("msg", "Player not found: "+params.getUtfString("name"));
                            send("sMsg", infos, sender);
                        }
                    }
                    else
                    {
                        ISFSObject infos = new SFSObject();
                        infos.putUtfString("msg", "Player not found: "+params.getUtfString("name"));
                        send("sMsg", infos, sender);
                    }
                }*/

                if(_cmd.Equals("equipItem"))
                {
                    Player myPlayer = sender;

                    myPlayer.myCharacter.equipItem(message.GetString(1), false);
                      
                }
		
                if(_cmd.Equals("unEquipItem"))
                {
                    Player myPlayer = sender;

                    if (!myPlayer.myCharacter.unEquipItem(message.GetString(1), false))
                    {
                        myPlayer.myCharacter.equipItem(message.GetString(1), false);
                    }
                }

                if (_cmd.Equals("req"))
                {
                    mainInstance.sendEntityInfos(sender, mainInstance.units[message.GetString(1)]);
                }

                if (_cmd.Equals("shop"))
                {
                    Player myPlayer = sender;
  
                }

                if (_cmd.Equals("useItem"))
                {
                    Player myPlayer = sender;
                    spellsInstance.UseItem(myPlayer.myCharacter, message.GetString(3), message.GetString(2), message.GetFloat(4), message.GetFloat(5), message.GetFloat(6)); 
                }
                
                if (_cmd.Equals("buyItem"))
                {
                    Player myPlayer = sender;

                    //TODO: Check if this item is sold by this npc using message.GetString(1)
                    sender.myCharacter.buyItem(message.GetString(2));
                    
                }

                if (_cmd.Equals("sellItem"))
                {
                    Player myPlayer = sender;
                    //TODO: Check if i am close to a vendor with message.GetString(1)
                    sender.myCharacter.sellItem(message.GetString(2));

                }

                if (_cmd.Equals("notifyNpc"))
                {
                    Player myPlayer = sender;

                    Entity myEntity = ((Entity)(mainInstance).units[message.GetString(1)]);

                    if (!myEntity.team.Equals(myPlayer.myCharacter.team) && myEntity.agressivity == AgressivityLevel.agressive && myEntity.focus == null)
                    {
                        myEntity.focus = myPlayer.myCharacter.id;
                    }

                }

                if (_cmd.Equals("pet_spells"))
                {
                    //TODO: do this in a way specific to my class or with a spell Master
                    Player myPlayer = sender;
                    Entity myEntity = ((Entity)(mainInstance).units[message.GetString(1)]);
                    if (myEntity.master.Equals(sender.myCharacter))
                    {
                        myEntity.sendSpells(myPlayer);
                    }
                }

                if (_cmd.Equals("cast"))
                {
                    Player myPlayer = sender;
                    Entity myEntity = ((Entity)(mainInstance).units[message.GetString(1)]);

                    if (myEntity.id.Equals(sender.ConnectUserId) || myEntity.master.Equals(sender.myCharacter))
                    {
                        if(myEntity.type==EntityType.player)
                            spellsInstance.UseSpell(((Entity)(mainInstance).units[message.GetString(1)]), message.GetString(3), message.GetString(2), message.GetFloat(4), message.GetFloat(5), message.GetFloat(6));
                        else
                            spellsInstance.IAUseSpell(((Entity)(mainInstance).units[message.GetString(1)]), message.GetString(3), message.GetString(2), message.GetFloat(4), message.GetFloat(5), message.GetFloat(6));

                    }
                    else
                    {
                        sender.Send("err", "e1");
                    }
                     
                }

                if (_cmd.Equals("stopCast"))
                {
                    Player myPlayer = sender;

                    Entity myEntity = ((Entity)(mainInstance).units[message.GetString(1)]);

                    if (myEntity.id.Equals(sender.ConnectUserId) || myEntity.master.Equals(sender.myCharacter))
                    {
                        try
                        {
                            if (((Entity)(mainInstance).units[message.GetString(1)]).canalisedSpell != null)
                                ((Entity)(mainInstance).units[message.GetString(1)]).canalisedSpell.Stop();
                            ((Entity)(mainInstance).units[message.GetString(1)]).canalisedSpell = null;
                        }
                        catch (Exception e)
                        {
                            sender.Send("err", "cancel failed at 1: " + e);
                        }

                        try
                        {
                            if (((Entity)(mainInstance).units[message.GetString(1)]).incantation != null)
                                ((Entity)(mainInstance).units[message.GetString(1)]).incantation.Stop();
                            ((Entity)(mainInstance).units[message.GetString(1)]).incantation = null;
                        }
                        catch (Exception e)
                        {
                            sender.Send("err", "cancel failed at 2: " + e);
                        }
                        
                        String[] data = {((Entity)(mainInstance).units[message.GetString(1)]).id};
                        mainInstance.sendDataToAll("stopCast", data, sender.myCharacter);
                    }
                    else
                    {
                        sender.Send("err", "e1");
                    }

                }
                
                if (_cmd.Equals("p"))
                {
                    Entity myCharacter = sender.myCharacter;

                    if (myCharacter.hp > 0)
                        myCharacter.setPos(message.GetFloat(1), message.GetFloat(2), message.GetFloat(3));

                    myCharacter.sendPos(new Vector3(0, 0, 0));

                    List<String> pList = new List<String>();
                    foreach (string s in mainInstance.units.Keys)
                    {
                        if (myCharacter.position.Substract(mainInstance.units[s].position).Magnitude() < mainInstance.baseRefSize)
                            pList.Add(s + "|x" + mainInstance.units[s].position.x + "y" + mainInstance.units[s].position.y + "z" + mainInstance.units[s].position.z);
                    }

                    sender.Send("ul", pList.ToArray());
                }

                if (_cmd.Equals("lp"))
                {
                    Entity myCharacter = mainInstance.units[message.GetString(8)];

                    if (myCharacter.master.Equals(sender.myCharacter) || myCharacter.Equals(sender.myCharacter))
                    {
                        if (myCharacter.hp > 0)
                            myCharacter.setPos(message.GetFloat(1), message.GetFloat(2), message.GetFloat(3));

                        myCharacter.sendLocalPos(new Vector3(message.GetFloat(4), message.GetFloat(5), message.GetFloat(6)), message.GetString(7));
                    }
                }

                if (_cmd.Equals("trigger"))
                {
                    if (mainInstance.units[message.GetString(1)].getDistance(sender.myCharacter.position)<5)
                        mainInstance.units[message.GetString(1)].myTrigger.activate(sender.myCharacter);
                    else
                        sender.Send("err", "s6");
                }

                if (_cmd.Equals("useUnit"))
                {
                    //mainInstance.units[message.GetString(1)].myTrigger.activate();
                }

                if (_cmd.Equals("money"))
                {
                    sender.myCharacter.sendMoney();
                }  

                if(_cmd.Equals("atk"))
                {
                    if (sender.myCharacter.getDistance(mainInstance.units[message.GetString(1)]) <= sender.myCharacter.infos.range)
                    {
                        if (sender.myCharacter.attackCounter > sender.myCharacter.getAttackSpeed())
                        {
                            sender.myCharacter.attack(message.GetString(1));
                            sender.myCharacter.attackCounter = 0;
                        }
                    }
                    else
                    {
                        sender.Send("err", "s6");
                    }
                }

                if (_cmd.Equals("invoke"))
                {
                    try
                    {
                        WorldInfos tmpInfos = mainInstance.worldInfos;
                        Entity invokedCreature = new Entity(mainInstance, "", "", tmpInfos.getEntityInfosByName(message.GetString(1)), sender.myCharacter.position.Add(new Vector3(1,0,1)));
                        invokedCreature.infos.baseSpeed = 2;
                        invokedCreature.master = sender.myCharacter;
                        float x = 1+(float)(mainInstance.mainSeed.NextDouble() * 5f);
                        if (mainInstance.mainSeed.Next(0, 11) > 5)
                            x = -x;
                        float y = 1 + (float)(mainInstance.mainSeed.NextDouble() * 5f);
                        if (mainInstance.mainSeed.Next(0, 11) > 5)
                            y = -y;

                        invokedCreature.petOffset = new Vector3(x, 0, y);

                        mainInstance.addUnit(invokedCreature);
                    }
                    catch(Exception e)
                    {
                        sender.Send("err", "Unit not found!");
                    }
                }

                if (_cmd.Equals("invokeFoe"))
                {
                    try
                    {
                        if (!message.GetString(1).Equals("clone"))
                        {
                            WorldInfos tmpInfos = mainInstance.worldInfos;
                            Entity invokedCreature = new Entity(mainInstance, "", "", tmpInfos.getEntityInfosByName(message.GetString(1)), sender.myCharacter.position.Add(new Vector3(1, 0, 1)));
                            invokedCreature.infos.baseSpeed = 2;
                            invokedCreature.team = "";
                            mainInstance.addUnit(invokedCreature);
                        }
                        else 
                        {
                            Entity author = sender.myCharacter;

                            EntityInfos clonedInfos = new EntityInfos(author.infos);

                            BaseStatsInfos myUnitBasStats = new BaseStatsInfos(author.infos.baseStats);

                            Entity mirrorUnit = new Entity(mainInstance, "", author.id, clonedInfos, author.position.Add(new Vector3(2, 0, 1)));
                            mirrorUnit.spells = author.spells;
                            mirrorUnit.infos.baseStats.agi += 100;
                            mirrorUnit.infos.range = 30;
                            mirrorUnit.isTemp = true;
                            mirrorUnit.team = "";
                            //mirrorUnit.items = ((Hero)author).equippedItems;
                            mainInstance.addUnit(mirrorUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        sender.Send("err", "Unit not found!");
                    }
                }

                if (_cmd.Equals("mount"))
                {
                    try
                    {
                        Entity tmpEntity = mainInstance.units[message.GetString(1)];
                        if (tmpEntity.ridable)
                        {
                            if (tmpEntity.master.Equals(sender.myCharacter))
                            {
                                sender.myCharacter.riding = tmpEntity;
                                sender.myCharacter.sendRider();
                            }
                            else
                            {
                                sender.Send("err", "This unit is not serving you!");
                            }
                        }
                        else
                        {
                            sender.Send("err", "You cannot mount this unit!");
                        }
                    }
                    catch (Exception e)
                    {
                        sender.myCharacter.riding = null;
                        sender.myCharacter.sendRider();
                    }
                }
	    }
    }
}

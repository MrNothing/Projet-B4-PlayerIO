﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace ProjetB4
{
    public enum QuestStatus
    { 
        started, completed
    }

    /// <summary>
    /// This is the actual quest object that defines its status
    /// </summary>
    public class Quest
    {
        /// <summary>
        /// this acts as id since there can only be one quest instance per quest.
        /// </summary>
        public string quest;
        public QuestStatus status = QuestStatus.started;

        public Dictionary<byte, QuestTask> tasks = new Dictionary<byte, QuestTask>();
        public Dictionary<QuestTaskType, List<QuestTask>> tasksByType = new Dictionary<QuestTaskType, List<QuestTask>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Quest"/> class.
        /// </summary>
        public Quest()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Quest"/> class.
        /// </summary>
        /// <param name="infos">The infos in Hashtable format generated by the toHashtable() method.</param>
        public Quest(Hashtable infos)
        {
            quest = infos["quest"].ToString();
            status = (QuestStatus)Enum.Parse(typeof(QuestStatus), infos["status"].ToString(), true);

            Hashtable tmpTasks = (Hashtable)infos["tasks"];
            
            foreach (string s in tmpTasks.Keys)
            {
                QuestTask myTask = new QuestTask(byte.Parse(s),(Hashtable)tmpTasks[s]);
                tasks.Add(byte.Parse(s), myTask);
            }
        }

        /// <summary>
        /// returns a generic Hashtable for my great pleasure.
        /// </summary>
        /// <returns></returns>
        public Hashtable toHashtable()
        {
            Hashtable tmpQuest = new Hashtable();
            tmpQuest.Add("quest", quest);
            tmpQuest.Add("status", status.ToString());

            Hashtable tmpTasks = new Hashtable();
            foreach (byte b in tasks.Keys)
            {
                tmpTasks.Add(b + "", tasks[b].toHashtable());
            }

            tmpQuest.Add("tasks", tmpTasks);

            return tmpQuest;
        }
    }
}
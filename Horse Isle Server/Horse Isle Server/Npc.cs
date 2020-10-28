﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horse_Isle_Server
{
    class Npc
    {
        public struct NpcReply
        {
            public int RandomId;
            public int Id;
            public string ReplyText;
            public int GotoChatpoint;
            public int RequiresQuestIdCompleted;
            public int RequiresQuestIdNotCompleted;
        }
        public struct NpcChat
        {
            public int Id;
            public string ChatText;
            public int ActivateQuestId;

            public NpcReply[] Replies;
        }
        public struct NpcEntry
        {
            public int Id;
            public string Name;
            public string AdminDescription;
            public string ShortDescription;
            public string LongDescription;
            public bool Moves;
            public int X;
            public int Y;
            public string StayOn;
            public int RequiresQuestIdCompleted;
            public int RequiresQuestIdNotCompleted;
            public string UDLRScript;
            public int UDLRStartX;
            public int UDLRStartY;
            public bool AdminOnly;
            public bool LibarySearchable;
            public int IconId;

            public NpcChat[] Chatpoints;
        }

        public static List<NpcEntry> NpcList = new List<NpcEntry>();

        public NpcReply GetNpcReply(NpcEntry npc, int randomid)
        {

            foreach (NpcChat chatpoint in npc.Chatpoints)
            {
                foreach (NpcReply reply in chatpoint.Replies)
                {
                    if (reply.RandomId == randomid)
                        return reply;
                }
            }
            throw new KeyNotFoundException("Npc reply with " + randomid + " not found!");
        }

        public static NpcReply GetNpcReply(NpcChat chatpoint, int randomid)
        {
            foreach(NpcReply reply in chatpoint.Replies)
            {
                if (reply.RandomId == randomid)
                    return reply;
            }
            throw new KeyNotFoundException("Npc reply with " + randomid + " not found!");
        }
        public static NpcEntry GetNpcById(int id)
        {
            foreach(NpcEntry npc in NpcList)
            {
                if (npc.Id == id)
                    return npc;
            }
            throw new KeyNotFoundException("Npc id: " + id + " not found!");
        }

        public static NpcEntry GetNpcByXAndY(int x, int y)
        {
            foreach(NpcEntry npc in NpcList)
            {
                if (npc.X == x && npc.Y == y)
                    return npc;
            }
            throw new KeyNotFoundException("Npc at X " + x + ", Y " + y + " not found!");
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;

namespace CommandUpdate
{
    ///<summary>
    ///将输入指令升级
    ///</summary>
    static public class CommandUpgrade
    {
        ///<summary>
        ///指令升级至1.13版本
        ///</summary>
        static public class to1_13Command
        {
            ///<summary>
            ///将输入指令升级至1.13版本
            ///</summary>
            ///<param name="input">输入指令</param>
            ///<returns>升级至1.13版本的指令</returns>
            static public string CommandConvert(string input)
            {
                string finalCommand = "";

                string[] parts = input.Split(new char[] { ' ' });

                for (int i = 0; i < parts.Length; i++)
                {
                    //<>
                    //about test✔
                    //<>
                    //execute✔ ：execute <entity> -> execute as <entity> execute at @s
                    //8.17 + 'then'
                    if (parts[i] == "execute" || parts[i] == "/execute")
                    {
                        if (i < parts.Length - 1)
                        {
                            //'then'
                            if (i < parts.Length - 5)
                            {
                                if (parts[i + 5] != "execute" && parts[i + 5] != "/execute" && parts[i + 5] != "detect" && parts[i + 5] != "/detect")
                                    parts[i + 5] = "then " + parts[i + 5];
                            }
                            parts[i + 1] = EntitySelector(parts[i + 1], null);
                            parts[i] = "execute as " + parts[i + 1] + " at @s";
                            parts = DelectArray(parts, i + 1);
                        }
                    }
                    //detect✔ ：detect <x> <y> <z> <blockId> [<dataTag>] -> detect <x> <y> <z> <blockId + [<dataTag>]>
                    //8.17 + 'then'
                    else if (parts[i] == "detect" || parts[i] == "/detect")
                    {
                        parts[i] = "execute if block";
                        //合并至方块格式
                        if (i < parts.Length - 5)
                        {
                            parts[i + 4] = BlockCombine(parts[i + 4], parts[i + 5], null);
                            //'then'
                            if (i < parts.Length - 6)
                            {
                                if (parts[i + 6] != "execute" && parts[i + 6] != "/execute" && parts[i + 6] != "detect" && parts[i + 6] != "/detect")
                                    parts[i + 6] = "then " + parts[i + 6];
                            }
                            parts = DelectArray(parts, i + 5);
                        }
                    }
                    //testforblock✔ ：testforblock <x> <y> <z> <blockId> [<dataTag>] [<nbt>] -> testforblock <x> <y> <z> <blockId + [<dataTag>] + [<nbt>]>
                    else if (parts[i] == "testforblock" || parts[i] == "/testforblock")
                    {
                        parts[i] = "execute if block";
                        //包含NBT
                        if (i < parts.Length - 6)
                        {
                            parts = CombineforNBT(parts, i + 6);
                            parts[i + 4] = BlockCombine(parts[i + 4], parts[i + 5], parts[i + 6]);
                            parts = DelectArray(parts, i + 6);
                            parts = DelectArray(parts, i + 5);
                        }
                        //包含dataTag
                        else if (i < parts.Length - 5)
                        {
                            parts[i + 4] = BlockCombine(parts[i + 4], parts[i + 5], null);
                            parts = DelectArray(parts, i + 5);
                        }
                    }
                    //testforblocks✔ ：testforblocks -> execute if blocks
                    else if (parts[i] == "testforblock" || parts[i] == "/testforblock")
                    {
                        parts[i] = "execute if blocks";
                    }
                    //function✔
                    else if (parts[i] == "function" || parts[i] == "/function")
                    {
                        if (i < parts.Length - 3)
                        {
                            parts[i + 3] = EntitySelector(parts[i + 3], null);
                            parts[i] = "execute " + parts[i + 2] + " entity " + parts[i + 3] + " function " + parts[i + 1];
                            parts = DelectArray(parts, i + 3);
                            parts = DelectArray(parts, i + 2);
                            parts = DelectArray(parts, i + 1);
                        }
                    }

                    //<>
                    //about blocks✔
                    //<>
                    //clone✔ ：clone <x1 y1 z1> <x2 y2 z2> <xt yt zt> filtered [force|move|normal] [<block>] [<data>] -> clone <x1 y1 z1> <x2 y2 z2> <xt yt zt> filtered [<block> + [<data>]] [force|move|normal]
                    //          clone <x1 y1 z1> <x2 y2 z2> <xt yt zt> [replace|masked] [force|move|normal] [<block>] [<data>] -> clone <x1 y1 z1> <x2 y2 z2> <xt yt zt> [replace|masked] [force|move|normal]
                    else if (parts[i] == "clone" || parts[i] == "/clone" && parts.Length > i + 10)
                    {
                        if (parts[i + 10] == "filter")
                        {
                            if (parts.Length == i + 13)
                            {
                                parts[i + 12] = BlockCombine(parts[i + 12], null, null);
                                SwapString(ref parts[i + 11], ref parts[i + 12]);
                            }
                            else if (parts.Length == i + 14)
                            {
                                parts[i + 12] = BlockCombine(parts[i + 12], parts[i + 13], null);
                                parts = DelectArray(parts, i + 13);
                                SwapString(ref parts[11], ref parts[i + 12]);
                            }
                        }
                        else if (parts[i + 10] == "replace" || parts[i + 10] == "masked")
                        {
                            if (parts.Length == i + 14)
                                parts = DelectArray(parts, i + 13);
                            else if (parts.Length == i + 13)
                                parts = DelectArray(parts, i + 12);
                        }
                    }
                    //fill✔ ：fill <x y z> <xt yt zt> <block> <data> replace [<replaceBlock>] [<replaceData>] -> fill <x y z> <xt yt zt> <block + [<data>]> replace [<filter> + [<data>]]
                    //         fill <x y z> <xt yt zt> <block> [<data>] [destroy|hollow|keep|outline|replace] [<nbt>] -> fill <x y z> <xt yt zt> <block + [<data>]> [destroy|hollow|keep|outline|replace]
                    else if (parts[i] == "fill" || parts[i] == "/fill" && parts.Length > i + 8)
                    {
                        //包含模式
                        if (parts.Length > i + 9)
                        {
                            if (parts[i + 9] == "replace")
                            {
                                if (parts.Length == i + 11)
                                {
                                    parts[i + 7] = BlockCombine(parts[i + 7], parts[i + 8], null);
                                    parts[i + 10] = BlockCombine(parts[i + 10], null, null);
                                    parts = DelectArray(parts, i + 8);
                                }
                                else if (parts.Length == i + 12)
                                {
                                    parts[i + 7] = BlockCombine(parts[i + 7], parts[i + 8], null);
                                    parts[i + 10] = BlockCombine(parts[i + 10], parts[i + 11], null);
                                    parts = DelectArray(parts, i + 11);
                                    parts = DelectArray(parts, i + 8);
                                }
                            }
                            else if (parts[i + 9] == "destroy" || parts[i + 9] == "hollow" || parts[i + 9] == "keep" || parts[i + 9] == "outline" || parts[i + 9] == "replace")
                            {
                                if (parts.Length >= i + 11)
                                {
                                    parts = CombineforNBT(parts, i + 10);
                                    parts[i + 7] = BlockCombine(parts[i + 7], parts[i + 8], parts[i + 10]);
                                    parts = DelectArray(parts, i + 10);
                                    parts = DelectArray(parts, i + 8);
                                }
                            }
                        }
                        //包含dataTag
                        else if (parts.Length == i + 9)
                        {
                            parts[i + 7] = BlockCombine(parts[i + 7], parts[i + 8], null);
                            parts = DelectArray(parts, i + 8);
                        }
                    }
                    //setblock✔ ：setblock <pos> <block> [<data>] [<mode>] [<nbt>] -> setblock <pos> <block + [<data>] + [<nbt>]> [<mode>]
                    else if (parts[i] == "setblock" || parts[i] == "/setblock" && parts.Length > i + 5)
                    {
                        //包含模式
                        if (parts.Length > i + 7)
                        {
                            parts = CombineforNBT(parts, i + 7);
                            parts[i + 4] = BlockCombine(parts[i + 4], parts[i + 5], parts[i + 7]);
                            parts = DelectArray(parts, i + 7);
                            parts = DelectArray(parts, i + 5);
                        }
                        //包含dataTag
                        else if (parts.Length > i + 5)
                        {
                            parts[i + 4] = BlockCombine(parts[i + 4], parts[i + 5], null);
                            parts = DelectArray(parts, i + 5);
                        }
                    }

                    //<>
                    //about items✔
                    //<>
                    //clear✔ ：clear [<target>] [<item>] [<data>] [<count>] [<nbt>] -> clear [<target>] [<item> + [<data>] + [<nbt>]] [<count>]
                    else if (parts[i] == "clear" || parts[i] == "/clear")
                    {
                        //包含NBT
                        if (parts.Length > i + 5)
                        {
                            parts[i + 1] = EntitySelector(parts[i + 1], null);
                            parts = CombineforNBT(parts, i + 5);
                            parts[i + 2] = ItemCombine(parts[i + 2], parts[i + 3], parts[i + 5]);
                            parts = DelectArray(parts, i + 5);
                            parts = DelectArray(parts, i + 3);
                        }
                        //包含dataTag
                        else if (parts.Length > i + 3)
                        {
                            parts[i + 1] = EntitySelector(parts[i + 1], null);
                            parts[i + 2] = ItemCombine(parts[i + 2], parts[i + 3], null);
                            parts = DelectArray(parts, i + 3);
                        }
                        //包含物品
                        else if (parts.Length > i + 2)
                        {
                            parts[i + 1] = EntitySelector(parts[i + 1], null);
                            parts[i + 2] = ItemCombine(parts[i + 2], null, null);
                        }
                        //包含对象
                        else if (parts.Length > i + 1)
                        {
                            parts[i + 1] = EntitySelector(parts[i + 1], null);
                        }
                    }
                    //give✔ ：give <players> <item> [<count>] [<data>] [<nbt>] -> give <players> <item + [<data>] + [<nbt>]> [<count>]
                    else if (parts[i] == "give" || parts[i] == "/give")
                    {
                        //包含NBT
                        if (parts.Length > i + 5)
                        {
                            parts[i + 1] = EntitySelector(parts[i + 1], null);
                            parts = CombineforNBT(parts, i + 5);
                            parts[i + 2] = ItemCombine(parts[i + 2], parts[i + 4], parts[i + 5]);
                            parts = DelectArray(parts, i + 5);
                            parts = DelectArray(parts, i + 4);
                        }
                        //包含dataTag
                        else if (parts.Length > i + 4)
                        {
                            parts[i + 1] = EntitySelector(parts[i + 1], null);
                            parts[i + 2] = ItemCombine(parts[i + 2], parts[i + 4], null);
                            parts = DelectArray(parts, i + 4);
                        }
                        //包含物品
                        else if (parts.Length > i + 2)
                        {
                            parts[i + 1] = EntitySelector(parts[i + 1], null);
                            parts[i + 2] = ItemCombine(parts[i + 2], null, null);
                        }
                        //包含对象
                        else if (parts.Length > i + 1)
                        {
                            parts[i + 1] = EntitySelector(parts[i + 1], null);
                        }
                    }
                    //replaceitem✔ ：replaceitem block <pos> <slot> <item> [<count>] [<data>] [<nbt>] -> replaceitem block <pos> <slot> <item + [<data>] + [<nbt>]> [<count>]
                    //                replaceitem entity <target> <slot> <item> [<count>] [<data>] [<nbt>] -> replaceitem entity <target> <slot> <item + [<data>] + [<nbt>]> [<count>]
                    else if (parts[i] == "replaceitem" || parts[i] == "/replaceitem")
                    {
                        //block
                        if (parts[i + 1] == "block")
                        {
                            //包含NBT
                            if (parts.Length > i + 9)
                            {
                                parts = CombineforNBT(parts, i + 9);
                                parts[i + 6] = ItemCombine(parts[i + 6], parts[i + 8], parts[i + 9]);
                                parts = DelectArray(parts, i + 9);
                                parts = DelectArray(parts, i + 8);
                            }
                            //包含dataTag
                            else if (parts.Length > i + 8)
                            {
                                parts[i + 6] = ItemCombine(parts[i + 6], parts[i + 8], null);
                                parts = DelectArray(parts, i + 8);
                            }
                            //包含物品
                            else if (parts.Length > i + 6)
                            {
                                parts[i + 6] = ItemCombine(parts[i + 6], null, null);
                            }
                        }
                        //entity
                        if (parts[i + 1] == "entity")
                        {
                            //包含NBT
                            if (parts.Length > i + 7)
                            {
                                parts[i + 2] = EntitySelector(parts[i + 2], null);
                                parts = CombineforNBT(parts, i + 7);
                                parts[i + 4] = ItemCombine(parts[i + 4], parts[i + 6], parts[i + 7]);
                                parts = DelectArray(parts, i + 7);
                                parts = DelectArray(parts, i + 6);
                            }
                            //包含dataTag
                            else if (parts.Length > i + 6)
                            {
                                parts[i + 2] = EntitySelector(parts[i + 2], null);
                                parts[i + 4] = ItemCombine(parts[i + 4], parts[i + 6], null);
                                parts = DelectArray(parts, i + 6);
                            }
                            //包含物品
                            else if (parts.Length > i + 4)
                            {
                                parts[i + 2] = EntitySelector(parts[i + 2], null);
                                parts[i + 4] = ItemCombine(parts[i + 4], null, null);
                            }
                            //包含对象
                            else if (parts.Length > i + 2)
                            {
                                parts[i + 2] = EntitySelector(parts[i + 2], null);
                            }
                        }
                    }

                    //<>
                    //about entities✔
                    //<>
                    //testfor✔ ：testfor <entity> [<nbt>] -> testfor <entity + [<nbt>]>
                    else if (parts[i] == "testfor" || parts[i] == "/testfor")
                    {
                        //包含NBT
                        if (i < parts.Length - 2)
                        {
                            parts = CombineforNBT(parts, i + 2);
                            parts[i + 1] = EntitySelector(parts[i + 1], parts[i + 2]);
                            parts[i] = "execute if entity " + parts[i + 1];
                            parts = DelectArray(parts, i + 2);
                            parts = DelectArray(parts, i + 1);
                        }
                        //包含实体
                        else if (i < parts.Length - 1)
                        {
                            parts[i + 1] = EntitySelector(parts[i + 1], null);
                            parts[i] = "execute if entity " + parts[i + 1];
                            parts = DelectArray(parts, i + 1);
                        }
                        else parts[i] = "execute if entity";
                    }
                    //effect✔ ：effect <entity> <effect> -> effect give <entity> <effect>
                    //           effect <entity> clear -> effect clear <entity>
                    else if (parts[i] == "effect" || parts[i] == "/effect")
                    {
                        //包含效果
                        if (i < parts.Length - 2)
                        {
                            parts[i + 1] = EntitySelector(parts[i + 1], null);
                            if (parts[i + 2] == "clear")
                            {
                                parts[i] = "effect clear";
                                parts = DelectArray(parts, i + 2);
                            }
                            else
                            {
                                parts[i] = "effect give";
                            }
                        }
                        //包含实体
                        else if (i < parts.Length - 1)
                        {
                            parts[i + 1] = EntitySelector(parts[i + 1], null);
                        }
                    }
                    //scoreboard✔ ：scoreboard players tag|set|remove|add <entity> <add|score> <tag|score> [<nbt>]
                    //               scoreboard players enable|list|reset|test <entity>
                    //               scoreboard players operation <entity> <objective> <operation> <entity> <objective>
                    //               scoreboard teams join <team> <entity> | leave <entity>
                    else if (parts[i] == "scoreboard" || parts[i] == "/scoreboard")
                    {
                        if (parts[i + 1] == "players")
                        {
                            if (i < parts.Length - 3 && (parts[i + 2] == "enable" || parts[i + 2] == "list" || parts[i + 2] == "reset" || parts[i + 2] == "test"))
                            {
                                parts[i + 3] = EntitySelector(parts[i + 3], null);
                            }
                            else if (i < parts.Length - 3 && parts[i + 2] == "operation")
                            {
                                if (i < parts.Length - 6) parts[i + 6] = EntitySelector(parts[i + 6], null);
                                if (i < parts.Length - 3) parts[i + 3] = EntitySelector(parts[i + 3], null);
                            }
                            else if (parts[i + 2] == "tag" || parts[i + 2] == "set" || parts[i + 2] == "add" || parts[i + 2] == "remove")
                            {
                                //包含NBT
                                if (i < parts.Length - 6)
                                {
                                    parts = CombineforNBT(parts, i + 6);
                                    parts[i + 3] = EntitySelector(parts[i + 3], parts[i + 6]);
                                    parts = DelectArray(parts, i + 6);
                                }
                                //包含实体
                                else if (i < parts.Length - 3)
                                {
                                    parts[i + 3] = EntitySelector(parts[i + 3], null);
                                }
                            }
                        }
                        else if (parts[i + 1] == "teams")
                        {
                            if (i < parts.Length - 4 && parts[i + 2] == "join")
                            {
                                parts[i + 4] = EntitySelector(parts[i + 4], null);
                            }
                            else if (i < parts.Length - 3 && parts[i + 2] == "leave")
                            {
                                parts[i + 3] = EntitySelector(parts[i + 3], null);
                            }
                        }
                    }
                    //xp✔ ：xp <level> [<entity>] -> experience add <players> <amount> [points|levels]
                    else if (parts[i] == "xp" || parts[i] == "/xp")
                    {
                        //包含实体
                        if (i < parts.Length - 2)
                        {
                            parts[i] = parts[i].Replace("xp", "experience");

                            if (Regex.Match(parts[i + 1], @"\d*[lL]$").Success)
                            {
                                string value = new Regex(@"[lL]$").Replace(parts[i + 1], "");
                                parts[i + 1] = EntitySelector(parts[i + 2], null) + " " + value + " levels";
                            }
                            else
                                parts[i + 1] = EntitySelector(parts[i + 2], null) + " " + parts[i + 1] + " points";
                            parts = DelectArray(parts, i + 2);
                        }
                        //包含等级
                    }
                    //stats✔ ：stats entity [<entity>] <stats> set [<entity>]
                    //          stats block <x> <y> <z> <stats> set [<entity>]
                    //          -> execute store result <name: entity> <objective: string>
                    //          -> execute store success <name: entity> <objective: string>
                    else if (parts[i] == "stats" || parts[i] == "/stats")
                    {
                        //包含实体
                        if (i < parts.Length - 2 && parts[i + 1] == "entity")
                        {
                            string target = "", mode = "", type = "";
                            target = EntitySelector(parts[i + 2], null);
                            //包含绑定
                            if (i < parts.Length - 4 && parts[i + 3] == "set")
                            {
                                mode = parts[i + 3];
                                if (parts[i + 4] == "AffectedBlocks" || parts[i + 4] == "AffectedEntities" || parts[i + 4] == "AffectedItems" || parts[i + 4] == "QueryResult")
                                {
                                    parts[i] = "execute store result";
                                    type = parts[i + 4];
                                }
                                else if (parts[i + 4] == "SuccessCount")
                                {
                                    parts[i] = "execute store success";
                                    type = parts[i + 4];
                                }
                                if (i < parts.Length - 5)
                                    parts[i + 5] = EntitySelector(parts[i + 5], null);

                                parts = DelectArray(parts, i + 4);
                                parts = DelectArray(parts, i + 3);
                                parts = DelectArray(parts, i + 2);
                                parts = DelectArray(parts, i + 1);
                            }
                            //包含清除
                            else if (i < parts.Length - 4 && parts[i + 3] == "clear")
                            {
                                mode = parts[i + 3];
                                if (parts[i + 4] == "AffectedBlocks" || parts[i + 4] == "AffectedEntities" || parts[i + 4] == "AffectedItems" || parts[i + 4] == "QueryResult")
                                {
                                    parts[i] = "execute store result";
                                    type = parts[i + 4];
                                }
                                else if (parts[i + 4] == "SuccessCount")
                                {
                                    parts[i] = "execute store success";
                                    type = parts[i + 4];

                                }
                                parts = DelectArray(parts, i + 4);
                                parts = DelectArray(parts, i + 3);
                                parts = DelectArray(parts, i + 2);
                                parts = DelectArray(parts, i + 1);
                            }
                            parts[i] = "#(target:" + target + ", mode:" + mode + ", type:" + type + ". you need to edit stats manually!) " + parts[i];
                        }
                        //包含方块
                        if (i < parts.Length - 4 && parts[i + 1] == "block")
                        {
                            string target = "", mode = "", type = "";
                            target = "[x=" + parts[i + 2] + ",y=" + parts[i + 3] + ",z=" + parts[i + 4] + "]";
                            //包含绑定
                            if (i < parts.Length - 6 && parts[i + 5] == "set")
                            {
                                mode = parts[i + 5];
                                if (parts[i + 6] == "AffectedBlocks" || parts[i + 6] == "AffectedEntities" || parts[i + 6] == "AffectedItems" || parts[i + 6] == "QueryResult")
                                {
                                    parts[i] = "execute store result";
                                    type = parts[i + 6];
                                }
                                else if (parts[i + 6] == "SuccessCount")
                                {
                                    parts[i] = "execute store success";
                                    type = parts[i + 6];
                                }
                                if (i < parts.Length - 7) parts[i + 7] = EntitySelector(parts[i + 7], null);
                                parts = DelectArray(parts, i + 6);
                                parts = DelectArray(parts, i + 5);
                                parts = DelectArray(parts, i + 4);
                                parts = DelectArray(parts, i + 3);
                                parts = DelectArray(parts, i + 2);
                                parts = DelectArray(parts, i + 1);
                            }
                            //包含清除
                            else if (i < parts.Length - 6 && parts[i + 5] == "clear")
                            {
                                mode = parts[i + 5];
                                if (parts[i + 6] == "AffectedBlocks" || parts[i + 6] == "AffectedEntities" || parts[i + 6] == "AffectedItems" || parts[i + 6] == "QueryResult")
                                {
                                    parts[i] = "execute store result";
                                    type = parts[i + 6];
                                }
                                else if (parts[i + 6] == "SuccessCount")
                                {
                                    parts[i] = "execute store success";
                                    type = parts[i + 6];
                                }
                                parts = DelectArray(parts, i + 6);
                                parts = DelectArray(parts, i + 5);
                                parts = DelectArray(parts, i + 4);
                                parts = DelectArray(parts, i + 3);
                                parts = DelectArray(parts, i + 2);
                                parts = DelectArray(parts, i + 1);
                            }
                            parts[i] = "#(target:" + target + ", mode:" + mode + ", type:" + type + ". you need to edit stats manually!) " + parts[i];
                        }
                    }
                    //enchant✔ ：enchant [<entity>]
                    else if (parts[i] == "enchant" || parts[i] == "/enchant")
                    {
                        //包含实体
                        if (i < parts.Length - 1)
                        {
                            parts[i + 1] = EntitySelector(parts[i + 1], null);
                        }
                    }
                    //entitydata✔ ：entitydata [<entity>]
                    else if (parts[i] == "entitydata" || parts[i] == "/entitydata")
                    {
                        //包含实体
                        if (i < parts.Length - 1)
                        {
                            parts[i + 1] = EntitySelector(parts[i + 1], null);
                        }
                    }
                    //advancement✔ ：advancement <grant|revoke|test> <entity>
                    else if (parts[i] == "advancement" || parts[i] == "/advancement")
                    {
                        //包含模式
                        if (i < parts.Length - 2 && (parts[i + 1] == "grant" || parts[i + 1] == "revoke" || parts[i + 1] == "test"))
                        {
                            parts[i + 2] = EntitySelector(parts[i + 2], null);
                        }
                    }
                    //ban✔ ：ban <entity>
                    else if (parts[i] == "ban" || parts[i] == "/ban")
                    {
                        //包含实体
                        if (i < parts.Length - 1)
                        {
                            parts[i + 1] = EntitySelector(parts[i + 1], null);
                        }
                    }
                    //ban-ip✔ ：ban-ip <entity>
                    else if (parts[i] == "ban-ip" || parts[i] == "/ban-ip")
                    {
                        //包含实体
                        if (i < parts.Length - 1)
                        {
                            parts[i + 1] = EntitySelector(parts[i + 1], null);
                        }
                    }
                    //banlist✔ ：banlist <entities>
                    else if (parts[i] == "banlist" || parts[i] == "/banlist")
                    {
                        //遍历实体
                        for (int j = i + 1; j < parts.Length; j++)
                        {
                            parts[j] = EntitySelector(parts[j], null);
                        }
                    }
                    //deop✔ ：deop <entity>
                    else if (parts[i] == "deop" || parts[i] == "/deop")
                    {
                        //包含实体
                        if (i < parts.Length - 1)
                        {
                            parts[i + 1] = EntitySelector(parts[i + 1], null);
                        }
                    }
                    //kick✔ ：kick <entity>
                    else if (parts[i] == "kick" || parts[i] == "/kick")
                    {
                        //包含实体
                        if (i < parts.Length - 1)
                        {
                            parts[i + 1] = EntitySelector(parts[i + 1], null);
                        }
                    }
                    //kill✔ ：kill <entity>
                    else if (parts[i] == "kill" || parts[i] == "/kill")
                    {
                        //包含实体
                        if (i < parts.Length - 1)
                        {
                            parts[i + 1] = EntitySelector(parts[i + 1], null);
                        }
                    }
                    //msg | tell✔ ：msg | tell <entity>
                    else if (parts[i] == "msg" || parts[i] == "/msg" || parts[i] == "tell" || parts[i] == "/tell")
                    {
                        //包含实体
                        if (i < parts.Length - 1)
                        {
                            parts[i + 1] = EntitySelector(parts[i + 1], null);
                        }
                    }
                    //op✔ ：op <entity>
                    else if (parts[i] == "op" || parts[i] == "/op")
                    {
                        //包含实体
                        if (i < parts.Length - 1)
                        {
                            parts[i + 1] = EntitySelector(parts[i + 1], null);
                        }
                    }
                    //pardon✔ ：pardon <entity>
                    else if (parts[i] == "pardon" || parts[i] == "/pardon")
                    {
                        //包含实体
                        if (i < parts.Length - 1)
                        {
                            parts[i + 1] = EntitySelector(parts[i + 1], null);
                        }
                    }
                    //pardon-ip✔ ：pardon-ip <entity>
                    else if (parts[i] == "pardon-ip" || parts[i] == "/pardon-ip")
                    {
                        //包含实体
                        if (i < parts.Length - 1)
                        {
                            parts[i + 1] = EntitySelector(parts[i + 1], null);
                        }
                    }
                    //playsound✔ ：playsound <id> <mode> [<entity>]
                    else if (parts[i] == "playsound" || parts[i] == "/playsound")
                    {
                        //包含实体
                        if (i < parts.Length - 3)
                        {
                            parts[i + 3] = EntitySelector(parts[i + 3], null);
                        }
                    }
                    //recipe✔ ：recipe <give|take> <entity>
                    else if (parts[i] == "recipe" || parts[i] == "/recipe")
                    {
                        //包含实体
                        if (i < parts.Length - 2)
                        {
                            parts[i + 2] = EntitySelector(parts[i + 2], null);
                        }
                    }
                    //say✔ ：say <entity>
                    else if (parts[i] == "say" || parts[i] == "/say")
                    {
                        //包含实体
                        if (i < parts.Length - 1)
                        {
                            parts[i + 1] = EntitySelector(parts[i + 1], null);
                        }
                    }
                    //spawnpoint✔ ：spawnpoint <entity>
                    else if (parts[i] == "spawnpoint" || parts[i] == "/spawnpoint")
                    {
                        //包含实体
                        if (i < parts.Length - 1)
                        {
                            parts[i + 1] = EntitySelector(parts[i + 1], null);
                        }
                    }
                    //spreadplayers✔ ：spreadplayers <x> <z> <spreadDistance> <maxRange> <respectTeams> <entity>
                    else if (parts[i] == "spreadplayers" || parts[i] == "/spreadplayers")
                    {
                        //包含实体
                        if (i < parts.Length - 6)
                        {
                            parts[i + 6] = EntitySelector(parts[i + 6], null);
                        }
                    }
                    //stopsound✔ ：stopsound <entity>
                    else if (parts[i] == "stopsound" || parts[i] == "/stopsound")
                    {
                        //包含实体
                        if (i < parts.Length - 1)
                        {
                            parts[i + 1] = EntitySelector(parts[i + 1], null);
                        }
                    }
                    //tp | teleport✔ ：tp | teleport <entity> [<entity>]
                    else if (parts[i] == "tp" || parts[i] == "/tp" || parts[i] == "teleport" || parts[i] == "/teleport")
                    {
                        //包含实体
                        if (i < parts.Length - 1)
                        {
                            parts[i + 1] = EntitySelector(parts[i + 1], null);
                        }
                        //包含目标实体
                        if (i < parts.Length - 2)
                        {
                            parts[i + 2] = EntitySelector(parts[i + 2], null);
                        }
                    }
                    //tellraw✔ ：tellraw <entity>
                    else if (parts[i] == "tellraw" || parts[i] == "/tellraw")
                    {
                        //包含实体
                        if (i < parts.Length - 1)
                        {
                            parts[i + 1] = EntitySelector(parts[i + 1], null);
                        }
                    }
                    //title✔ ：title <entity>
                    else if (parts[i] == "title" || parts[i] == "/title")
                    {
                        //包含实体
                        if (i < parts.Length - 1)
                        {
                            parts[i + 1] = EntitySelector(parts[i + 1], null);
                        }
                    }
                    //whitelist✔ ：whitelist remove|add <entity>
                    else if (parts[i] == "whitelist" || parts[i] == "/whitelist")
                    {
                        //包含实体
                        if (i < parts.Length - 2 && (parts[i + 1] == "remove" || parts[i + 1] == "add"))
                        {
                            parts[i + 2] = EntitySelector(parts[i + 2], null);
                        }
                    }

                    //<>
                    //about string change✔
                    //<>
                    //gamemode | defaultgamemode✔ ：gamemode | defaultgamemode <mode> [<entity>]
                    else if (parts[i] == "gamemode" || parts[i] == "/gamemode" || parts[i] == "defaultgamemode" || parts[i] == "/defaultgamemode")
                    {
                        //包含模式
                        if (i < parts.Length - 1)
                        {
                            if (parts[i + 1] == "0" || parts[i + 1] == "s") parts[i + 1] = "survival";
                            if (parts[i + 1] == "1" || parts[i + 1] == "c") parts[i + 1] = "creative";
                            if (parts[i + 1] == "2" || parts[i + 1] == "a") parts[i + 1] = "adventure";
                            if (parts[i + 1] == "3" || parts[i + 1] == "sp") parts[i + 1] = "spectator";
                            //包含实体
                            if (i < parts.Length - 2)
                            {
                                parts[i + 2] = EntitySelector(parts[i + 2], null);
                            }
                        }
                    }
                    //difficulty✔ ：difficulty <difficulty>
                    else if (parts[i] == "difficulty" || parts[i] == "/difficulty")
                    {
                        //包含模式
                        if (i < parts.Length - 1)
                        {
                            if (parts[i + 1] == "0" || parts[i + 1] == "p") parts[i + 1] = "peaceful";
                            if (parts[i + 1] == "1" || parts[i + 1] == "e") parts[i + 1] = "easy";
                            if (parts[i + 1] == "2" || parts[i + 1] == "n") parts[i + 1] = "normal";
                            if (parts[i + 1] == "3" || parts[i + 1] == "h") parts[i + 1] = "hard";
                        }
                    }
                    //toggledownfall✔ ：weather clear
                    else if (parts[i] == "toggledownfall" || parts[i] == "/toggledownfall")
                    {
                        parts[i] = "weather clear";
                    }
                }
                finalCommand = String.Join(" ", parts).Trim();
                return finalCommand;
            }
            ///<summary>
            ///批量操作,将输入的每条指令升级至1.13版本
            ///</summary>
            ///<param name="input">输入所有指令</param>
            ///<returns>升级至1.13版本的所有指令</returns>
            static public string BatchConvert(string input)
            {
                string[] commands = input.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < commands.Length; i++)
                {
                    commands[i] = CommandConvert(commands[i]);
                }
                input = String.Join(Environment.NewLine, commands);
                return input;
            }

            ///<summary>
            ///获取1.13版本的方块格式
            ///</summary>
            ///<param name="id">方块id,若无填null</param>
            ///<param name="data">方块数据,若无填null</param>
            ///<param name="nbt">方块nbt,若无填null</param>
            static public string BlockCombine(string id, string data, string nbt)
            {
                string block = id;
                if (data != null) { block = block + '[' + data + ']'; }
                if (nbt != null) { block = block + nbt; }
                return block;
            }
            ///<summary>
            ///获取1.13版本的物品格式
            ///</summary>
            ///<param name="id">物品id,若无填null</param>
            ///<param name="data">物品数据,若无填null</param>
            ///<param name="nbt">物品nbt,若无填null</param>
            static public string ItemCombine(string id, string data, string nbt)
            {
                string item = id;
                string[] nbtF = { "", "" };
                if (nbt != null) { nbtF[0] = new Regex(@"(?i)(?<=\{)(.*)(?=\})").Match(nbt).Value; }
                if (data != null && data != "") { nbtF[1] = "Damage:" + data + "s"; }
                if (nbtF[1] != "" && nbtF[0] != "") item = id + "{" + nbtF[0] + "," + nbtF[1] + "}";
                else if (nbtF[1] != "" && nbtF[0] == "") item = id + "{" + nbtF[1] + "}";
                else item = id;
                return item;
            }
            ///<summary>
            ///获取1.13版本的选择器格式
            ///</summary>
            ///<param name="input">实体选择器</param>
            ///<param name="nbt">实体nbt,若无填null</param>
            static public string EntitySelector(string input, string nbt)
            {
                string output = "";
                input = input.Trim();

                //分离目标选择器变量 outer
                string[] fp = input.Split(new char[] { '[' });
                string outer = fp[0];

                //分离目标选择器参数 elements[]
                if (input.Contains('[') && input.Contains(']'))
                {
                    string inside = new Regex(@"(?i)(?<=\[)(.*)(?=\])").Match(input).Value;
                    string[] elements = inside.Split(new char[] { ',' });

                    //分离参数和值
                    Dictionary<string, string> selectors = new Dictionary<string, string>();
                    foreach (string element in elements)
                    {
                        string[] sp = element.Split(new char[] { '=' });
                        try { selectors.Add(sp[0], sp[1]); } catch { }

                    }

                    //转化后的元素列表
                    ArrayList newele = new ArrayList();

                    //gamemode
                    if (selectors.ContainsKey("m"))
                    {
                        if (selectors["m"] == "0" || selectors["m"] == "s") selectors["m"] = "survival";
                        if (selectors["m"] == "1" || selectors["m"] == "c") selectors["m"] = "creative";
                        if (selectors["m"] == "2" || selectors["m"] == "a") selectors["m"] = "adventure";
                        if (selectors["m"] == "3" || selectors["m"] == "sp") selectors["m"] = "spectator";
                        if (selectors["m"] == "!0" || selectors["m"] == "!s") selectors["m"] = "!survival";
                        if (selectors["m"] == "!1" || selectors["m"] == "!c") selectors["m"] = "!creative";
                        if (selectors["m"] == "!2" || selectors["m"] == "!a") selectors["m"] = "!adventure";
                        if (selectors["m"] == "!3" || selectors["m"] == "!sp") selectors["m"] = "!spectator";
                        if (selectors["m"] == "-1") selectors.Remove("m");

                        if (selectors.ContainsKey("m"))
                            newele.Add("gamemode" + "=" + selectors["m"]);
                    }
                    //level
                    if (selectors.ContainsKey("l") || selectors.ContainsKey("lm"))
                    {
                        string[] level = { "", "" };
                        if (selectors.ContainsKey("lm")) level[0] = selectors["lm"];
                        if (selectors.ContainsKey("l")) level[1] = selectors["l"];
                        newele.Add("level" + "=" + level[0] + ".." + level[1]);
                    }
                    //distance
                    if (selectors.ContainsKey("r") || selectors.ContainsKey("rm"))
                    {
                        string[] distance = { "", "" };
                        if (selectors.ContainsKey("rm")) distance[0] = selectors["rm"];
                        if (selectors.ContainsKey("r")) distance[1] = selectors["r"];
                        newele.Add("distance" + "=" + distance[0] + ".." + distance[1]);
                    }
                    //x_rotation
                    if (selectors.ContainsKey("rx") || selectors.ContainsKey("rxm"))
                    {
                        string[] x_rotation = { "", "" };
                        if (selectors.ContainsKey("rxm")) x_rotation[0] = selectors["rxm"];
                        if (selectors.ContainsKey("rx")) x_rotation[1] = selectors["rx"];
                        newele.Add("x_rotation" + "=" + x_rotation[0] + ".." + x_rotation[1]);
                    }
                    //y_rotation
                    if (selectors.ContainsKey("ry") || selectors.ContainsKey("rym"))
                    {
                        string[] y_rotation = { "", "" };
                        if (selectors.ContainsKey("rym")) y_rotation[0] = selectors["rym"];
                        if (selectors.ContainsKey("ry")) y_rotation[1] = selectors["ry"];
                        newele.Add("y_rotation" + "=" + y_rotation[0] + ".." + y_rotation[1]);
                    }
                    Dictionary<string, string> scores = new Dictionary<string, string>();
                    //score_
                    foreach (string Key in selectors.Keys)
                    {
                        //key:score_name(_min), selectors[key]:score, score_name:scoreboard name;
                        //scores:{score_name:score}
                        //max
                        if (Key.Contains("score_") && !Key.Contains("_min"))
                        {
                            string score_name = new Regex(@"(?<=score_)(.*)").Match(Key).Value;
                            if (scores.ContainsKey(score_name))
                            {
                                scores[score_name] = scores[score_name].Replace("..", "");
                                if (scores[score_name] != selectors[Key])
                                    scores[score_name] = scores[score_name] + ".." + selectors[Key];
                            }
                            else scores.Add(score_name, ".." + selectors[Key]);
                        }
                        //min
                        else if (Key.Contains("score_") && Key.Contains("_min"))
                        {
                            string score_name = new Regex(@"(?i)(?<=score_)(.*)(?=_min)").Match(Key).Value;
                            if (scores.ContainsKey(score_name))
                            {
                                scores[score_name] = scores[score_name].Replace("..", "");
                                if (scores[score_name] != selectors[Key])
                                    scores[score_name] = selectors[Key] + ".." + scores[score_name];
                            }
                            else scores.Add(score_name, selectors[Key] + "..");
                        }
                    }
                    foreach (string score in scores.Keys)
                    {
                        newele.Add("score_" + score + "=" + scores[score]);
                    }
                    //limit
                    if (selectors.ContainsKey("c"))
                    {
                        if (Convert.ToInt32(selectors["c"]) >= 0)
                            newele.Add("limit" + "=" + selectors["c"]);
                        else
                            newele.Add("limit" + "=" + (-Convert.ToInt32(selectors["c"])).ToString() + ",sort=furthest");
                    }
                    //x
                    if (selectors.ContainsKey("x"))
                        newele.Add("x" + "=" + (Convert.ToInt32(selectors["x"]) + 0.5).ToString());
                    //z
                    if (selectors.ContainsKey("z"))
                        newele.Add("z" + "=" + (Convert.ToInt32(selectors["z"]) + 0.5).ToString());
                    //name
                    if (selectors.ContainsKey("name"))
                        newele.Add("name" + "=\"" + selectors["name"] + "\"");

                    //others
                    foreach (string Key in selectors.Keys)
                    {
                        if (!Key.Contains("score_") && !Key.Contains("_min") && Key != "m" && Key != "lm" && Key != "l" && Key != "rm" && Key != "r" && Key != "rx" && Key != "rxm" && Key != "ry" && Key != "rym" && Key != "x" && Key != "z" && Key != "c" && Key != "name")
                            newele.Add(Key + "=" + selectors[Key]);
                    }

                    //@r
                    if (outer == "@r")
                    {
                        if (selectors.ContainsKey("type"))
                            outer = "@e";
                        else
                            outer = "@a";
                        newele.Add("sort=random");
                    }

                    //nbt
                    if (nbt != null && nbt != "")
                    {
                        newele.Add("nbt=" + nbt);
                    }

                    //合并所有元素
                    output = String.Join(",", (string[])newele.ToArray(typeof(string)));
                }
                //选择器为@r,sort增加项
                else if (outer == "@r")
                {
                    outer = "@a";
                    output = "sort=random";
                    if (nbt != null && nbt != "")
                    {
                        output = output + ",nbt=" + nbt;
                    }
                }
                //选择器为空的nbt单独处理
                else if (nbt != null && nbt != "")
                {
                    output = "nbt=" + nbt;
                }
                //合并outer
                if (output != "")
                    output = outer + "[" + output + "]";
                else
                    output = outer;

                return output;
            }
            //交换字符
            static private void SwapString(ref string str1, ref string str2)
            {
                string Tmp = str1;
                str1 = str2;
                str2 = Tmp;
            }
            //删除元素
            static private string[] DelectArray(string[] strings, int i)
            {
                ArrayList array = new ArrayList(strings);
                array.RemoveAt(i);
                strings = (string[])array.ToArray(typeof(string));
                return strings;
            }
            //结合NBT（防止NBT中出现空格影响结果）
            static private string[] CombineforNBT(string[] strings, int i)
            {
                string[] final = strings;
                for (int index = i + 1; index < strings.Length; index++)
                {
                    final[i] = final[i] + ' ' + final[index];
                }
                for (int index = strings.Length - 1; index > i; index--)
                {
                    final = DelectArray(final, index);
                }

                var match = Regex.Matches(final[i], @"((?i)(@[earp]\[.*\])" + "|@[earp])");
                //对nbt中的选择器进行转化
                for (int j = 0; j < match.Count; j++)
                {

                    final[i] = final[i].Replace(match[j].Value, EntitySelector(match[j].Value, null));
                }
                return final;
            }
        }
        ///<summary>
        ///将输入指令升级至1.11版本
        ///</summary>
        static public class to1_11Command
        {
            ///<summary>
            ///将输入指令升级至1.11版本
            ///</summary>
            ///<param name="input">输入指令</param>
            ///<returns>升级至1.11版本的指令</returns>
            static public string CommandConvert(string input)
            {
                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("item", "Item"); dict.Add("xp_orb", "XPOrb"); dict.Add("area_effect_cloud", "AreaEffectCloud"); dict.Add("elder_guardian", "ElderGuardian"); dict.Add("wither_skeleton", "WitherSkeleton"); dict.Add("stray", "Stray"); dict.Add("egg", "ThrownEgg"); dict.Add("leash_knot", "LeashKnot"); dict.Add("painting", "Painting"); dict.Add("arrow", "Arrow"); dict.Add("snowball", "Snowball"); dict.Add("fireball", "Fireball"); dict.Add("small_fireball", "SmallFireball"); dict.Add("ender_pearl", "ThrownEnderpearl"); dict.Add("eye_of_ender_signal", "EyeOfEnderSignal"); dict.Add("potion", "ThrownPotion"); dict.Add("xp_bottle", "ThrownExpBottle"); dict.Add("item_frame", "ItemFrame"); dict.Add("wither_skull", "WitherSkull"); dict.Add("tnt", "PrimedTnt"); dict.Add("falling_block", "FallingSand"); dict.Add("fireworks_rocket", "FireworksRocketEntity"); dict.Add("husk", "Husk"); dict.Add("spectral_arrow", "SpectralArrow"); dict.Add("shulker_bullet", "ShulkerBullet"); dict.Add("dragon_fireball", "DragonFireball"); dict.Add("zombie_villager", "ZombieVillager"); dict.Add("skeleton_horse", "SkeletonHorse"); dict.Add("zombie_horse", "ZombieHorse"); dict.Add("armor_stand", "ArmorStand"); dict.Add("donkey", "Donkey"); dict.Add("mule", "Mule"); dict.Add("commandblock_minecart", "MinecartCommandBlock"); dict.Add("boat", "Boat"); dict.Add("minecart", "MinecartRideable"); dict.Add("chest_minecart", "MinecartChest"); dict.Add("furnace_minecart", "MinecartFurnace"); dict.Add("tnt_minecart", "MinecartTNT"); dict.Add("hopper_minecart", "MinecartHopper"); dict.Add("spawner_minecart", "MinecartSpawner"); dict.Add("creeper", "Creeper"); dict.Add("skeleton", "Skeleton"); dict.Add("spider", "Spider"); dict.Add("giant", "Giant"); dict.Add("zombie", "Zombie"); dict.Add("slime", "Slime"); dict.Add("ghast", "Ghast"); dict.Add("zombie_pigman", "PigZombie"); dict.Add("enderman", "Enderman"); dict.Add("cave_spider", "CaveSpider"); dict.Add("silverfish", "Silverfish"); dict.Add("blaze", "Blaze"); dict.Add("magma_cube", "LavaSlime"); dict.Add("ender_dragon", "EnderDragon"); dict.Add("wither", "WitherBoss"); dict.Add("bat", "Bat"); dict.Add("witch", "Witch"); dict.Add("endermite", "Endermite"); dict.Add("guardian", "Guardian"); dict.Add("shulker", "Shulker"); dict.Add("pig", "Pig"); dict.Add("sheep", "Sheep"); dict.Add("cow", "Cow"); dict.Add("chicken", "Chicken"); dict.Add("squid", "Squid"); dict.Add("wolf", "Wolf"); dict.Add("mooshroom", "MushroomCow"); dict.Add("snowman", "SnowMan"); dict.Add("ocelot", "Ozelot"); dict.Add("villager_golem", "VillagerGolem"); dict.Add("horse", "Horse"); dict.Add("rabbit", "Rabbit"); dict.Add("polar_bear", "PolarBear"); dict.Add("villager", "Villager"); dict.Add("ender_crystal", "EnderCrystal");
                foreach (string key in dict.Keys)
                {
                    if (input.Contains("type=" + dict[key]) || input.Contains("id:" + dict[key]) || input.Contains("summon " + dict[key]))
                    {
                        input = input.Replace("type=" + dict[key], "type=" + key);
                        input = input.Replace("id:" + dict[key], "id:" + key);
                        input = input.Replace("summon " + dict[key], "summon " + key);
                    }
                }
                return input;
            }
            ///<summary>
            ///批量操作,将输入的每条指令升级至1.11版本
            ///</summary>
            ///<param name="input">输入所有指令</param>
            ///<returns>升级至1.11版本的所有指令</returns>
            static public string BatchConvert(string input)
            {
                string[] commands = input.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < commands.Length; i++)
                {
                    commands[i] = CommandConvert(commands[i]);
                }
                input = String.Join(Environment.NewLine, commands);
                return input;
            }
        }
        ///<summary>
        ///将输入指令升级至最新版本(1.13)
        ///</summary>
        static public class helicopterCommand
        {
            ///<summary>
            ///将输入指令升级至最新版本(目前为1.13)
            ///</summary>
            ///<param name="input">输入指令</param>
            ///<returns>升级至最新版本的指令</returns>
            static public string CommandConvert(string input)
            {
                input = CommandUpgrade.to1_11Command.CommandConvert(input);
                input = CommandUpgrade.to1_13Command.CommandConvert(input);
                return input;
            }
            ///<summary>
            ///将输入全部指令批量升级至最新版本(目前为1.13)
            ///</summary>
            ///<param name="input">输入全部指令</param>
            ///<returns>升级至最新版本的全部指令</returns>
            static public string BatchConvert(string input)
            {
                string[] commands = input.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < commands.Length; i++)
                {
                    commands[i] = CommandConvert(commands[i]);
                }
                input = String.Join(Environment.NewLine, commands);
                return input;
            }
        }
    }
}

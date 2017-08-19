# MinecraftCommandUpdate
Update your minecraft command
# How to use

**namespace**: 
```using CommandUpdate;```

## **to 1.13**: 
> The changes see here : [preview of 1.13 commands](https://www.reddit.com/user/Dinnerbone/comments/6l6e3d/a_completely_incomplete_super_early_preview_of/), Last updated: 2017-08-17.
- **Convert a command to 1.13**: 
```string result = CommandUpgrade.to1_13Command.CommandConvert(string input);```
###### example:
```
input = "/execute @r[score_good_min=1] ~ ~ ~ say hi"
result = "execute as @a[score_good=1..,sort=random] at @s ~ ~ ~ then say hi"
```
- **Convert a batch of commands to 1.13**:
```string result = CommandUpgrade.to1_13Command.BatchConvert(string input);```
###### example:
```
input = @"/execute @r[score_good_min=1] ~ ~ ~ say hi
          scoreboard players operation @a[c=1,m=2] score1 = @e[x=0,y=0,z=0,r=1] score2"
result = @"execute as @a[score_good=1..,sort=random] at @s ~ ~ ~ then say hi
          scoreboard players operation @a[gamemode=adventure,limit=1] score1 = @e[distance=..1,x=0.5,z=0.5,y=0] score2"
```
- **Convert argument type of blocks**:
```string result = CommandUpgrade.to1_13Command.BlockCombine(string blockID, string blockData, string blockNBT);```
###### example:
```
blockID = "minecraft:chest", blockData = "facing=north", blockNBT = "{Items:[{Slot:0b,id:"minecraft:apple",Count:1b,Damage:0s}]}";
result = "minecraft:chest[facing=north]{Items:[{Slot:0b,id:"minecraft:apple",Count:1b,Damage:0s}]}";
```
- **Convert argument type of items**:
```string result = CommandUpgrade.to1_13Command.ItemCombine(string itemID, string itemData, string itemNBT);```
###### example:
```
itemID = "minecraft:apple", itemData = "0", itemNBT = "{display:{Name:"my apple"}}";
result = "minecraft:apple{display:{Name:"my apple"},Damage:0s}";
```
- **Convert argument type of entity selectors**:
```string result = CommandUpgrade.to1_13Command.EntitySelector(string entitySelector, string entityNBT);```
###### example:
```
entitySelector = "@e[type=sheep,name=jeb_,score_health_min=1,score_health=7,c=1]", entityNBT = "{OnGround:1b}";
result = "@e[score_health=1..7,limit=1,name=\"jeb_\",type=sheep,nbt={OnGround:1b}]";
```


## **to 1.11**: 
> The changes are about entities' type , ep: `Bat -> bat`, `Armorstand -> armor_stand`.
- **Convert a command to 1.11**:
```string result = CommandUpgrade.to1_11Command.CommandConvert(string input);```
###### example:
```
input = "testfor @e[type=Armorstand]";
result = "testfor @e[type=armor_stand]";
```
- **Convert a batch of commands to 1.11**:
```string result = CommandUpgrade.to1_11Command.BatchConvert(string input);```
###### example:
```
input = @"testfor @e[type=Armorstand]
          testfor @e[type=Sheep]";
result = "testfor @e[type=armor_stand]
          testfor @e[type=sheep]";
```

## **helicopterCommand**: 
> Directly convert 1.13(or lower) commands  to 1.13.
- **Convert a command**:
```string result = CommandUpgrade.helicopterCommand.CommandConvert(string input);```
- **Convert a batch of commands**:
```string result = CommandUpgrade.helicopterCommand.BatchConvert(string input);```

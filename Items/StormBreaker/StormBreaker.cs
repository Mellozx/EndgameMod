using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace EndgameMod.Items.StormBreaker
{
    public class StormBreaker : ModItem // StormBreaker thingy **UNCOMPLETE**
    {

        public int MellosMultiuse = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Breaker");
        }

        public override void SetDefaults()
        {
            item.width = 38;
            item.height = 38;

            item.damage = 88;
            item.knockBack = 4f;

            item.thrown = true;
            item.noMelee = true;
            item.noUseGraphic = true;

            item.useAnimation = 14;
            item.useTime = 14;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;

            item.value = Item.sellPrice(0, 10, 0, 0);
            item.rare = ItemRarityID.Red;

            item.shoot = mod.ProjectileType("StormBreakerProj");
            item.shootSpeed = 13f;
        }

        public override bool AltFunctionUse(Player player) => true; // basically means you can use the item both left click and right click.




        public override bool CanUseItem(Player player)
            => player.ownedProjectileCounts[ProjectileType<StormBreakerProj>()] < 1;
        // {
        // 
        // 	{
        //         if (player.ownedProjectileCounts[mod.ProjectileType("StormBreakerProj")] >= 1)
        //         {
        //             return false;
        //         }
        //     }
        // 	return true;
        // }



        public override void AddRecipes() // just a test recipe *not final*
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.HallowedBar, 25);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}

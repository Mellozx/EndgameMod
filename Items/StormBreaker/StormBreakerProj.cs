using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EndgameMod.Items.StormBreaker
{
    public class StormBreakerProj : ModProjectile // Simple Boomerang Projectile, Needs to Shoot Lightning and be faster in later development.
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Breaker");
        }
        

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;

            projectile.friendly = true;
            projectile.thrown = true;

            projectile.penetrate = 6;

            projectile.aiStyle = 3;
            projectile.timeLeft = 3600;
            aiType = 52;
        }

    }
}
        


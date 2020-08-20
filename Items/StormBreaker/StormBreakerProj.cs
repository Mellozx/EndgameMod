using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace EndgameMod.Items.StormBreaker
{
    public class StormBreakerProj : ModProjectile // Simple Boomerang Projectile + Animation
    {

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Breaker");

        }


        public override void SetDefaults()
        {
            Main.projFrames[projectile.type] = 22;           
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.thrown = true;
            projectile.penetrate = 6;
            projectile.aiStyle = 3;
            projectile.timeLeft = 3600;
            aiType = 52;
        }

        public override bool PreDraw(SpriteBatch sb, Color lightColor) // funky time
        {
            if ((projectile.frameCounter = ++projectile.frameCounter % 1) == 0) projectile.frame = ++projectile.frame % 22;
            return true;

        }
    }
}
        


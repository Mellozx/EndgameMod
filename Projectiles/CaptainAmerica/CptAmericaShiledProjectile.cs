using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using System.Collections.Generic;
using System;

namespace EndgameMod.Projectiles.CaptainAmerica
{
	public class CptAmericaShiledProjectile : ModProjectile
	{
		private int bounces;

		// Refactor AI.
		// Fix Sprite Sizing Issues
		// Proper Bounce.

		public override string Texture => "EndgameMod/Projectiles/CaptainAmerica/CaptainAmericaShieldProjectile";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Captain America's Shield");

			Main.projFrames[projectile.type] = 4;
		}

		public override void SetDefaults()
		{
			projectile.damage = 70;
			projectile.friendly = true;

			projectile.penetrate = 10;

			projectile.width = 4;
			projectile.height = 4;

			drawOffsetX = -30;
			drawOriginOffsetY = -4;
		}

		public override bool? CanCutTiles() => true;

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
			bounces++;
			if (bounces > 3) { projectile.Kill(); }

			if (projectile.velocity.X != oldVelocity.X) { projectile.velocity.X = -oldVelocity.X; }
			if (projectile.velocity.Y != oldVelocity.Y) { projectile.velocity.Y = -oldVelocity.Y; }
			
			return false;
		}

		public override void AI()
		{
			projectile.ai[0] += 1f;
			
			// Animation, ~~idk if this would work or not.~~ somehow this works.
			if ((projectile.frameCounter = ++projectile.frameCounter % 4) == 0)
			{ projectile.frame = ++projectile.frame % Main.projFrames[projectile.type]; }

			projectile.velocity.Y += 0.1f;
			projectile.direction = projectile.spriteDirection = projectile.velocity.X > 0f ? 1 : -1;
			projectile.rotation = projectile.velocity.ToRotation();
			if (projectile.spriteDirection == -1) projectile.rotation += (float)Math.PI;

			// Hitboxes, funn.
			if (projectile.spriteDirection == 1)
			{
				drawOffsetX = -30;
				drawOriginOffsetX = 14;
			}
			else
			{
				drawOffsetX = -2;
				drawOriginOffsetX = -14;
			}

			// Kill projectile if far from player.
			float projDistanceFromPlayer = (projectile.position - Main.player[projectile.owner].position).Length();
			if (projDistanceFromPlayer > 120 * 16 || projectile.ai[0] > 5 * 60f) { projectile.Kill(); }
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			// Revamp this like seriously it looks auful right now.
			// NOPE NOPE, no thanks, not yet, this looks bad and i can't put my finger on how to fix it.
			/* 
			if (projectile.frameCounter > 0) return true;
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (projectile.oldSpriteDirection[0] == -1) spriteEffects = SpriteEffects.FlipHorizontally;
			
			Texture2D shieldTexture = Main.projectileTexture[projectile.type];
			int frameHeight = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
			int startY = frameHeight;
			Rectangle sourceRect = new Rectangle(0, startY, shieldTexture.Width, frameHeight);
			Vector2 origin = sourceRect.Size() / 2f;
			Color drawColor = projectile.GetAlpha(lightColor);
			drawColor.A = 0x07;

			Vector2 oldProjScreenPos = projectile.oldPosition - Main.screenPosition + new Vector2(0f, projectile.gfxOffY);

			Main.spriteBatch.Draw(shieldTexture, 
				oldProjScreenPos, sourceRect, drawColor, projectile.oldRot[0],
				origin, projectile.scale, spriteEffects, 0f);

			projectile.oldRot[0] = projectile.rotation;
			*/

			return true;
		}
	}
}

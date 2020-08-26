using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace EndgameMod.Items.InfinityGauntlet
{
	public class PowerStoneBeamProjectile : ModProjectile
	{
		// Charge Time in ticks.
		private const float CHARGETIME = 30f;

		private const float MAXDISTANCE = 1000f;
		private const int POSOFFSET = 38;


		public float Distance { get => projectile.ai[0]; set => projectile.ai[0] = value; }
		public float Charge { get => projectile.localAI[0]; set => projectile.localAI[0] = value; }

		public bool IsAtMaxCharge => Charge == CHARGETIME;


		public override string Texture => "EndgameMod/Items/InfinityGauntlet/ExampleLaser";
		
		public override bool ShouldUpdatePosition() => false;
		public override void SetStaticDefaults() =>	DisplayName.SetDefault("Power Stone Beam");

		public override void SetDefaults()
		{
			projectile.width = 20;
			projectile.height = 20;

			projectile.friendly = true;

			projectile.penetrate = -1;
			projectile.tileCollide = false;

			projectile.magic = true;
			projectile.hide = true;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			if (IsAtMaxCharge)
				DrawLaser(spriteBatch, Main.projectileTexture[projectile.type], Main.player[projectile.owner].Center,
					projectile.velocity, 10, projectile.damage, -1.57f, color: Color.White, offsetDist: POSOFFSET);
			return false;
		}


		private void DrawLaser(SpriteBatch spriteBatch, Texture2D texture, Vector2 start, Vector2 unit, float stepDist, 
			int damage, float rotation = 0f, float scale = 1f, Color color = default(Color), 
			int offsetDist = 50)
		{
			float rot = unit.ToRotation() + rotation;

			int frameCount = 3;
			int frameHeight = texture.Height / frameCount;
			int startY = frameHeight;
			int txtrWidth = texture.Width;

			// Body
			for (float i = offsetDist; i < Distance; i += stepDist)
			{
				Color c = color;
				Vector2 origin = start + (i * unit);
				spriteBatch.Draw(texture, origin - Main.screenPosition, new Rectangle(0, startY, txtrWidth, frameHeight),
					i < offsetDist ? Color.Transparent : c, rot, new Vector2(txtrWidth * 0.5f, frameHeight * 0.5f),
					scale, 0, 0);
			}

			// Tail
			spriteBatch.Draw(texture, start + unit * (offsetDist - stepDist) - Main.screenPosition,
				new Rectangle(0, startY * 0, txtrWidth, frameHeight), color, rot, 
				new Vector2(txtrWidth * 0.5f, frameHeight * 0.5f), scale, 0, 0);

			// Head
			spriteBatch.Draw(texture, start + (Distance + stepDist) * unit - Main.screenPosition,
				new Rectangle(0, startY * 2, txtrWidth, frameHeight), color, rot,
				new Vector2(txtrWidth * 0.5f, frameHeight * 0.5f), scale, 0, 0.1f);
		}


		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			if (!IsAtMaxCharge) return false;

			Player player = Main.player[projectile.owner];
			Vector2 unit = projectile.velocity;
			float point = 0f;
			return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), player.Center,
				player.Center + unit * Distance, 16, ref point);
		}


		public override void CutTiles()
		{
			DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
			Vector2 unit = projectile.velocity;
			Utils.PlotTileLine(projectile.Center, projectile.Center + unit * Distance, (projectile.width + 16) * projectile.scale, DelegateMethods.CutTiles);
		}


		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{ target.immune[projectile.owner] = 3; }


		public override void AI()
		{
			// Planned mechanic

			// - While charging.
			// Charge Laser.
			// While charging spawn dusts going towards gauntlet from a bit away.

			// - Upon full charge.
			// Fire laser.
			// Spawn Dust Randomly outwards.

			// - Upon colliding
			// Spawn Dusts going outwards for spilling laser energy.

			// Rework Collision Detection.
			// Redesign Tail sprite.
			// Redesign Head Sprite.

			// Make laser bigger in general.

			// ---------------


			// - Projectile Updates - //
			Player _player = Main.player[projectile.owner];
			Vector2 mouseDir = Main.MouseWorld - _player.Center;
			mouseDir.Normalize();

			projectile.velocity = mouseDir;
			projectile.direction = Main.MouseWorld.X > _player.Center.X ? 1 : -1;
			projectile.position = _player.Center + (projectile.velocity * POSOFFSET);
			projectile.netUpdate = true;

			// - Player Updates - //
			_player.ChangeDir(projectile.direction);
			_player.heldProj = projectile.whoAmI;
			_player.itemTime = 2;
			_player.itemAnimation = 2;
			_player.itemRotation = (float)Math.Atan2(projectile.velocity.Y * projectile.direction, 
				projectile.velocity.X * projectile.direction);

			// - Charge - //

			if (!_player.channel) { projectile.Kill(); return; }
			if (Charge < CHARGETIME)
			{
				Charge++;

				// Spawn Dusts going towards gauntlet
				float projDrawOffset = POSOFFSET - Main.projectileTexture[projectile.type].Height / 3;
				if (Charge > 10)
				{
					for (int i = 0; i < 2; i++)
					{
						float shootRot = projectile.velocity.ToRotation();
						float randDir = shootRot + MathHelper.ToRadians(Main.rand.Next(-50, 50));
						float randDist = Main.rand.NextFloat(8, 18);
						Vector2 randPos = _player.Center + new Vector2((float)Math.Cos(randDir) * randDist,
							(float)Math.Sin(randDir) * randDist) + (projectile.velocity * projDrawOffset);

						Vector2 velToGauntlet = -(randPos -
							(new Vector2(_player.Center.X + Main.rand.Next(-5, 5), _player.Center.Y + Main.rand.Next(-5, 5))
							+ (projectile.velocity * projDrawOffset))) / 6;

						// Dust chargeDust = Main.dust[Dust.NewDust(randPos, 0, 0, DustID.PurpleCrystalShard, velToGauntlet.X,
						// 	velToGauntlet.Y, Main.rand.Next(0x44, 0x88), Color.Purple, 0.7f)];
						Dust chargeDust = Dust.NewDustPerfect(randPos, DustID.PurpleCrystalShard, velToGauntlet,
							Main.rand.Next(0x44, 0x88), Color.Purple, 0.6f);
						chargeDust.noGravity = true;
						// Main.NewText(randDir.ToString(), Color.White);
					}
				}
			}

			if (!IsAtMaxCharge) return;

			// ----- After Charged ----- //

			// - Set Laser End Position. - //
			for (Distance = POSOFFSET; Distance <= MAXDISTANCE; Distance += 5)
			{
				Vector2 end = _player.Center + (projectile.velocity * Distance);
				if (!Collision.CanHitLine(_player.Center, 1, 1, end, 1, 1)) 
				{ Distance -= 5f; break; }
			}

			// --- Spawn Dusts --- //

			// Spawn Dust where projectile hits.
			Vector2 dustPos = _player.Center + (projectile.velocity * Distance);
			// Idk if this is neccessary but I'll put it here just in case checking whether or not a block exists before
			// calling HitTiles saves some extra proccessing. Remove later if deemed unnecessary
			// if (Distance < MAXDISTANCE || 
			//	(Distance >= MAXDISTANCE && !Collision.CanHit(dustPos, 1, 1, dustPos + projectile.velocity, 1, 1)))
			// { Collision.HitTiles(dustPos + projectile.velocity, projectile.velocity, projectile.width, projectile.height); }

			// 'Power Spill' Dusts
			for (int i = 0; i < 2; i++)
			{
				float randDir = projectile.velocity.ToRotation() + MathHelper.ToRadians(Main.rand.NextFloat(-10, 10));
				float randVel = (float)(Main.rand.NextDouble() * 8) + 3;
				Vector2 dustVel = new Vector2((float)Math.Cos(randDir) * randVel, (float)Math.Sin(randDir) * randVel);
				Dust laserDust = Main.dust[Dust.NewDust(dustPos, 0, 0, DustID.PurpleCrystalShard, dustVel.X, dustVel.Y,
					Main.rand.Next(0x78), Color.Purple, 1.2f)];
				laserDust.noGravity = true;
			}


			// Spawn Dust in front of gauntlet
			Vector2 gauntletDustPos = _player.Center + (projectile.velocity * 
				(POSOFFSET - Main.projectileTexture[projectile.type].Height / 3));
			for (int i = 0; i < 2; i++)
			{
				float randDir = projectile.velocity.ToRotation() + MathHelper.ToRadians(Main.rand.NextFloat(-20, 20));
				float randVel = (float)(Main.rand.NextDouble() * 2.3) + 3;
				Vector2 dustVel = new Vector2((float)Math.Cos(randDir) * randVel, (float)Math.Sin(randDir) * randVel);
				Dust gauntletDust = Main.dust[Dust.NewDust(gauntletDustPos, 0, 0, DustID.PurpleCrystalShard, 
					dustVel.X, dustVel.Y, Main.rand.Next(0x40), Color.Purple, 0.9f)];
				gauntletDust.noGravity = true;
			}

			// --- --- //

			// - Emit Light. - //
			DelegateMethods.v3_1 = new Vector3(0.8f, 0.8f, 1f);
			Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * (Distance - POSOFFSET), 26, DelegateMethods.CastLight);

			// ----- ----- ----- ----- //
			// Old code.
			// Player player = Main.player[projectile.owner];
			// projectile.timeLeft = 2;

			// This is just copy paste from ExampleLaser to get the basic laser working, 
			// rework later.
			// -> Rework made, needs further testing before can commit as usable.
			// UpdatePlayer(player);
			// ChargeLaser(player);
			// 
			// if (Charge < CHARGETIME) return;
			// 
			// SetLaserPosition(player);
			// SpawnDusts(player);
			// CastLights();
		}

		private void UpdatePlayer(Player player)
		{
			// Vector2 mousePos = Main.MouseWorld - player.Center;
			// mousePos.Normalize();
			// projectile.velocity = mousePos;
			// projectile.direction = Main.MouseWorld.X > player.position.X ? 1 : -1;
			// projectile.netUpdate = true;
			// projectile.position = player.Center + projectile.velocity * POSOFFSET;


			// int dir = projectile.direction;
			// player.ChangeDir(projectile.direction);
			// player.heldProj = projectile.whoAmI;
			// player.itemTime = 2;
			// player.itemAnimation = 2;
			// player.itemRotation = (float)Math.Atan2(projectile.velocity.Y * dir, projectile.velocity.X * dir);
		}

		private void ChargeLaser(Player player)
		{
			// if (!player.channel)
			// { projectile.Kill(); }
			// else
			// {
			// 	  if (Charge < CHARGETIME) Charge++;
			// }
		}

		private void SetLaserPosition(Player player)
		{
			// for (Distance = POSOFFSET; Distance <= MAXDISTANCE; Distance += 5f)
			// {
			// 	var start = player.Center + projectile.velocity * Distance;
			// 	if (!Collision.CanHit(player.Center, 1, 1, start, 1, 1))
			// 	{
			// 		Distance -= 5f;
			// 		break;
			// 	}
			// }
		}

		private void SpawnDusts(Player player)
		{
			// Vector2 unit = projectile.velocity * -1;
			// Vector2 dustPos = player.Center + projectile.velocity * Distance;
			// // some math i dont exactly know, i asked tmodcord and just put -1 as a setting standard and it worked 
			// 
			// for (int i = 0; i < 2; ++i)
			// {
			// 	   // Random Velocity Math.
			// 	   float num1 = projectile.velocity.ToRotation();//+ (Main.rand.Next(2) == 1 ? -1.0f : 1.0f) * 1.57f;
			// 	   float num2 = (float)(Main.rand.NextDouble() * 0.8f + 1.0f);
			// 	   Vector2 dustVel = new Vector2((float)Math.Cos(num1) * num2, (float)Math.Sin(num1) * num2);
			// 	   
			// 	   Dust laserDust = Main.dust[Dust.NewDust(dustPos, 0, 0, DustID.Electric, dustVel.X, dustVel.Y)];
			// 	   
			// 	   laserDust.noGravity = true;
			// 	   laserDust.scale = 1.2f;
			// 	   
			// 	   float scatterDustRot = projectile.velocity.ToRotation() + MathHelper.ToRadians(Main.rand.Next(-5, 6));
			// 	   float velRand = Main.rand.NextFloat(40);
			// 	   Vector2 postLaserDustVel = new Vector2((float)Math.Cos(scatterDustRot) * velRand, (float)Math.Sin(scatterDustRot) * velRand);
			// 	   laserDust = Dust.NewDustDirect(dustPos, 0, 0, DustID.Electric, postLaserDustVel.X, postLaserDustVel.Y);
			// 	   laserDust.fadeIn = 0f;
			// 	   laserDust.noGravity = true;
			// 	   laserDust.scale = 1.2f;
			// 	   laserDust.color = Color.Purple;
			// }
		}

		private void CastLights()
		{
			// Cast a light along the line of the laser
			// DelegateMethods.v3_1 = new Vector3(0.8f, 0.8f, 1f);
			// Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * (Distance - POSOFFSET), 26, DelegateMethods.CastLight);
		}
	}
}

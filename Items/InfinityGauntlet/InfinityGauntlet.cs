using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace EndgameMod.Items.InfinityGauntlet
{
	class GauntletModeInfo
	{
		private GauntletModeInfo(string name, string desc, Color color)
		{ Name = name; Description = desc; Color = color; }	

		// Stored Values
		public string Name { get; private set; }
		public string Description { get; private set; }
		public Color Color { get; private set; }

		// So basically... this gets it by index. Yeah...
		static string[] IndexToPropName = new string[7] { "Power", "Space", "Reality", "Soul", "Time", "Mind", "Snap" };
		public static GauntletModeInfo GetByIndex(int index)
			=> typeof(GauntletModeInfo).GetProperty(IndexToPropName[index]).GetGetMethod().Invoke(null, null) as GauntletModeInfo;

		// Mimicing an enum but stores multiple values.
		public static GauntletModeInfo Power 
			=> new GauntletModeInfo("Power", "<<Power>> Your Power? it's Infinite.", new Color(0x80, 0x00, 0x80));
		public static GauntletModeInfo Space
			=> new GauntletModeInfo("Space", "<<Space>> All that roams in Space is yours.", new Color(0x1E, 0x90, 0xFF));
		public static GauntletModeInfo Reality
			=> new GauntletModeInfo("Reality", "<<Reality>> Reality is yours to reshape.", new Color(0xDC, 0x14, 0x3C));
		public static GauntletModeInfo Soul
			=> new GauntletModeInfo("Soul", "<<Soul>> A Soul for a Soul.", new Color(0xFF, 0x7F, 0x50));
		public static GauntletModeInfo Time
			=> new GauntletModeInfo("Time", "<<Time>> The Future? The Past? The Present? You have control over all of them.", new Color(0x7F, 0xFF, 0x00));
		public static GauntletModeInfo Mind
			=> new GauntletModeInfo("Mind", "<<Mind>> Your Knowledge knows no limits.", new Color(0xFF, 0xFF, 0x66));
		public static GauntletModeInfo Snap
			=> new GauntletModeInfo("Snap", "<<Judgement>> The lives in the universe is yours to judge.", Color.White);
	}

	[AutoloadEquip(EquipType.HandsOn)]
	public class InfinityGauntlet : ModItem
	{
		public int GauntletMode = 0;
		public int pushTimer = 0;
		public int MellosMultiuse = 0;


		private MouseState oldMouseState;
		private MouseState newMouseState;
		private bool rightMouseDown;


		private int[] itemCounter = new int[2];
		private Vector2[] oldPlayerPositions = new Vector2[60];

		private bool isRewinding;


		public override bool CloneNewInstances => true;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Infinity Gauntlet");
			Tooltip.SetDefault("The power of the universe in the palm of your hand.");
		}

		public override void SetDefaults()
		{
			item.damage = 50;
			item.knockBack = 6;

			item.width = 40;
			item.height = 40;

			item.noUseGraphic = true;
			item.useTime = 20;
			item.useAnimation = 20;
			item.useStyle = ItemUseStyleID.HoldingUp;

			item.value = Item.buyPrice(0, 50, 0, 0);
			item.rare = ItemRarityID.Purple;

			item.UseSound = SoundID.Item1;
			item.shoot = ProjectileType<PowerStoneBeamProjectile>();
			item.shootSpeed = 1;

			// Fills rewind position data so it doesn't glitch out.
			for (int i = 0; i < oldPlayerPositions.Length; i++)
			{ oldPlayerPositions[i] = new Vector2(Main.spawnTileX, Main.spawnTileY); }
		}



		public override bool AltFunctionUse(Player player) => true;

		public override void HoldItem(Player player)
		{
			oldMouseState = newMouseState;
			newMouseState = Mouse.GetState();
			rightMouseDown = oldMouseState.RightButton == ButtonState.Pressed && newMouseState.RightButton == ButtonState.Pressed;
		}

		public override void UpdateInventory(Player player)
		{
			// Rewind Script
			if (!isRewinding)
			{
				itemCounter[0]++;
				if ((itemCounter[0] %= 5 * 60 / oldPlayerPositions.Length) == 0)
				{
					Array.Copy(oldPlayerPositions, 0, oldPlayerPositions, 1, oldPlayerPositions.Length - 1); // Shift back one.
					oldPlayerPositions[0] = player.position; // Insert new.
				}
			}
			else UpdatePlayerRewind(player);
		}

		public override bool CanUseItem(Player player)
		{
			// Mode switcher
			if (player.altFunctionUse == 2)
			{
				if (rightMouseDown) return false;

				item.useStyle = ItemUseStyleID.HoldingUp;
				
				GauntletMode++;
				GauntletMode %= 7;

				Main.PlaySound(SoundLoader.customSoundType, -1, -1, mod.GetSoundSlot(SoundType.Custom, "Sounds/InfinityStoneSound"));
				Main.NewText(GauntletModeInfo.GetByIndex(GauntletMode).Name, GauntletModeInfo.GetByIndex(GauntletMode).Color);
				switch (GauntletMode)
				{
					// Lmao why do i keep finding duplicate code that does the same thing.
					// Also this should probably get reworked soon.
					case 0:
						item.useTime = 8; item.useAnimation = 16; break;
					case 1:
						item.useTime = 8; item.useAnimation = 16; break;
					case 2:
						item.useTime = 20; item.useAnimation = 20; break;
					case 3:
						item.useTime = 8; item.useAnimation = 16; break;
					case 4:
						item.useTime = 2; item.useAnimation = 16; break;
					case 5:
						item.useTime = 8; item.useAnimation = 16; break;
					case 6: break;
						// change use time later when all else is put into place.
				}
			}
			else
			{
				// 0Power, 1Space, 2Reality, 3Soul, 4Time, 5Mind, 6Snap
				if (player.whoAmI == Main.myPlayer /*&& Main.netMode != NetmodeID.Server*/)
				{
					if (GauntletMode >= 1) { TurnOffPowerBeam(); }
					if (GauntletMode == 0) { PowerStoneBeam(); }
					else if (GauntletMode == 1) { SpaceStonePushWave(player); }
					else if (GauntletMode == 2) { AlterNearbyReality(player); }
					else if (GauntletMode == 3) { ReapNearbySouls(player); }
					else if (GauntletMode == 4 && !isRewinding) { PlayerStateRewind(player); }
					else if (GauntletMode == 5) { /* MIND */ }
					else if (GauntletMode == 6) { /* DECIMATE */ }
				}
			}
			return true;
		}

		private void TurnOffPowerBeam() { item.channel = false; }

		private void PowerStoneBeam()
		{
			item.noMelee = true;
			item.magic = true;
			item.channel = true;
			item.useStyle = ItemUseStyleID.HoldingOut;
			item.shoot = ProjectileType<PowerStoneBeamProjectile>();
			item.shootSpeed = 1;
		}


		private void SpaceStonePushWave(Player player)
		{
			List<NPC> nearbyNPCs = new List<NPC>();
			for (int i = 0; i < 100; i++)
			{
				NPC _npc = Main.npc[i];
				if ((_npc.position - player.position).Length() < 50 * 16) nearbyNPCs.Add(_npc);
			}
			foreach (NPC npc in nearbyNPCs)
			{
				Vector2 npcDirFromPlayer = npc.position - player.position;
				npcDirFromPlayer.Normalize();
				npc.velocity =  npcDirFromPlayer * 50;
			}
			// Play cool explosion shockwave thing here

		}


		private void AlterNearbyReality(Player player)
		{
			List<Projectile> nearbyProj = new List<Projectile>();
			for (int i = 0; i < 100; i++)
			{
				Projectile _proj = Main.projectile[i];
				if ((_proj.position - player.position).Length() < 80 * 16 && (_proj.hostile || _proj.trap))
				{ nearbyProj.Add(_proj); }
			}
			foreach (Projectile proj in nearbyProj)
			{
				// spawn bubble at projectile position with projectile velocity.
				Dust.NewDust(proj.position, 10, 10, DustID.BubbleBlock, Main.rand.Next(2, 6), Main.rand.Next(2, 6), 0, Color.Aqua, 1);
				proj.Kill();
			}
			// Again, play cool animation shockwave thing here.

		}


		private void PlayerStateRewind(Player player)
		{
			// Initializes Rewind Sequence, isRewinding is set to tell game to continue rewind on next update,
			// and counter 0 is reset so delay works:
			isRewinding = true;
			itemCounter[0] = 0;
			UpdatePlayerRewind(player);
		}

		private void UpdatePlayerRewind(Player player)
		{
			const int DELAY = 2;
			itemCounter[0]++;
			player.velocity = Vector2.Zero;
			player.position = oldPlayerPositions[(int)Math.Floor((decimal)(itemCounter[0] / DELAY))];
			if (!(isRewinding = !(itemCounter[0] == (oldPlayerPositions.Length - 1) * DELAY)))
			{ for (int i = 0; i < oldPlayerPositions.Length; i++) oldPlayerPositions[i] = player.position; }
		}


		private void ReapNearbySouls(Player player)
		{
			List<NPC> nearbyEnemies = new List<NPC>();
			for (int i = 0; i < 100; i++)
			{
				NPC _npc = Main.npc[i];
				if ((_npc.position - player.position).Length() < 60 * 16 && !_npc.friendly)
				{ nearbyEnemies.Add(_npc); }
			}
			foreach (NPC enemy in nearbyEnemies)
			{
				if (Main.rand.NextFloat() < .375f) Item.NewItem(enemy.getRect(), ItemID.SoulofLight);
				else if (Main.rand.NextFloat() < .375f) Item.NewItem(enemy.getRect(), ItemID.SoulofNight);
				enemy.StrikeNPC(9999, 0, 0, false, true, false);
			}
		}




		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			string baseTooltip = "The Power of the Universe in the palm of your hand";
			GauntletModeInfo modeInfo = GauntletModeInfo.GetByIndex(GauntletMode);
			string textColor = BitConverter.ToString(new byte[] { modeInfo.Color.R, modeInfo.Color.G, modeInfo.Color.B }).Replace("-", "");
			string text = $"[c/{textColor}:{modeInfo.Description}]";
			
			foreach (TooltipLine line in tooltips)
			{ if (line.mod == "Terraria" && line.Name.StartsWith("Tooltip")) { line.text = $"{baseTooltip} {text}"; } }
		}


		public override void MeleeEffects(Player player, Rectangle hitbox) // not necessary could be removed
		{
			if (Main.rand.NextBool(3)) // Ok but what does this do really?
			{
				if (player.altFunctionUse == 2)
				{
					int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 169, 0f, 0f, 100, default(Color), 2f);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].velocity.X += player.direction * 2f;
					Main.dust[dust].velocity.Y += 0.2f;
				}
				else
				{
					int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.Fire, player.velocity.X * 0.2f + (float)(player.direction * 3), player.velocity.Y * 0.2f, 100, default(Color), 2.5f);
					Main.dust[dust].noGravity = true;
				}
			}
		}



		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			Vector2 speed = new Vector2(new Vector2(speedX, speedY).Length() * (speedX > 0 ? 1 : -1), 0);
			speedX = speed.X;
			speedY = speed.Y;
			return true;
		}
	}
}
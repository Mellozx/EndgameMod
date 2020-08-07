using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace EndgameMod.Items.InfinityGauntlet
{
	public class InfinityGauntlet : ModItem
	{

		public int GauntletMode = 0;
		public int pushTimer = 0;
		public int MellosMultiuse = 0;



		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Infinity Gauntlet");
			Tooltip.SetDefault("This weapon is not to be wielded from the weak");
		}

		public override void SetDefaults()
		{
			item.damage = 50;
			item.melee = true;
			item.width = 40;
			item.height = 40;
			item.useTime = 20;
			item.useAnimation = 20;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.knockBack = 6;
			item.value = 10000;
			item.rare = ItemRarityID.Green;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.shoot = ProjectileType<ExampleLaser>();
			item.shootSpeed = 14f;
		}



		public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public override bool CanUseItem(Player player) 
		{
			if (player.altFunctionUse == 2) // NEEDS TO BE SLOWED DOWN
			{
				item.mana = 0;
				GauntletMode++;
				if (GauntletMode >= 6)
				{
					GauntletMode = 0;
				}
				Main.PlaySound(SoundLoader.customSoundType, -1, -1, mod.GetSoundSlot(SoundType.Custom, "Sounds/InfinityStoneSound"));
				string text = "";
				switch (GauntletMode)
				{
					case 0:
						text = "Power";

						item.useTime = 8;
						item.useAnimation = 16;
						break;
					case 1:
						text = "Space";

						item.useTime = 8;
						item.useAnimation = 16;
						break;
					case 2:
						text = "Reality";

						item.useTime = 20;
						item.useAnimation = 20;
						break;
					case 3:
						text = "Soul";

						item.useTime = 8;
						item.useAnimation = 16;
						break;
					case 4:
						text = "Time";

						item.useTime = 2;
						item.useAnimation = 16;
						break;
					case 5:
						text = "Mind";

						item.useTime = 8;
						item.useAnimation = 16;
						break;
					default:
						return base.CanUseItem(player);
				}
				Main.NewText(text, Color.White.R, Color.White.G, Color.White.B);
			}

			else // this code is 5% complete also needs to work with every other stone other than it being power for all modes.
			{
				item.damage = 40;
				item.noMelee = true;
				item.magic = true;
				item.channel = true; //Channel so that you can hold the weapon [Important]
				item.mana = 0;
				item.rare = ItemRarityID.Pink;
				item.width = 28;
				item.height = 30;
				item.useTime = 20;
				Main.PlaySound(SoundLoader.customSoundType, -1, -1, mod.GetSoundSlot(SoundType.Custom, "Sounds/InfinityStoneSound")); // CHANGE ASAP
				item.useStyle = ItemUseStyleID.HoldingOut;
				item.shootSpeed = 14f;
				item.useAnimation = 20;
				item.shoot = ProjectileType<ExampleLaser>();
				item.value = Item.sellPrice(silver: 3);

			}
			return base.CanUseItem(player);
		}




		public override void MeleeEffects(Player player, Rectangle hitbox) // not necessary could be removed
		{
			if (Main.rand.NextBool(3))
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

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack) // also unnecessary spaghetti
		{
			// Fix the speedX and Y to point them horizontally.
			speedX = new Vector2(speedX, speedY).Length() * (speedX > 0 ? 1 : -1);
			speedY = 0;
			// Add random Rotation
			Vector2 speed = new Vector2(speedX, speedY);
			speed = speed.RotatedByRandom(MathHelper.ToRadians(30));
			// Change the damage since it is based off the weapons damage and is too high
			damage = (int)(damage * .1f);
			speedX = speed.X;
			speedY = speed.Y;
			return true;
		}
	}
}
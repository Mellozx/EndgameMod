using EndgameMod.Projectiles.CaptainAmerica;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace EndgameMod.Items.CaptainAmerica
{
	public class CptAmericaShield : ModItem
	{
		public override string Texture => "EndgameMod/Items/CaptainAmerica/CaptainAmerica";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Captain America's Shield");
			Tooltip.SetDefault("I can do this all day");
		}

		public override void SetDefaults()
		{
			item.damage = 70;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.ranged = true;
			item.noUseGraphic = true;
			item.shoot = ProjectileType<CptAmericaShiledProjectile>();
			item.shootSpeed = 20f;
			item.useTurn = true;
			item.useTime = 10;
			item.useAnimation = 10;
			item.noMelee = true;
		}

		public override bool OnlyShootOnSwing => true;

		public override bool CanUseItem(Player player)
			=> player.ownedProjectileCounts[ProjectileType<CptAmericaShiledProjectile>()] < 1;

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{


			return base.Shoot(player, ref position, ref speedX, ref speedY, ref type, ref damage, ref knockBack);
		}

	}
}

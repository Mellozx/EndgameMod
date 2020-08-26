using EndgameMod.Items.CaptainAmerica;
using EndgameMod.Projectiles.CaptainAmerica;
using EndgameMod.Items.InfinityGauntlet;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace EndgameMod
{
	public sealed partial class EndModPlayer : ModPlayer
	{


		public override void FrameEffects()
		{
			if(player.HeldItem.type == ItemType<InfinityGauntlet>())
			{ player.handon = (sbyte)mod.GetEquipSlot("InfinityGauntlet", EquipType.HandsOn); }

			if (player.HeldItem.type == ItemType<CptAmericaShield>() && 
				player.ownedProjectileCounts[ProjectileType<CptAmericaShiledProjectile>()] < 1)
			{  }
		}
	}
}

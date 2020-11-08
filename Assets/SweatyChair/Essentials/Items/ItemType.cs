namespace SweatyChair
{

	/// <summary>
	/// The item type used for serializable item. Normally coresponding to a (ItemType)Item class, e.g. CoinItem, EnergyItem.
	/// A item can be either a cost (consume sth), or reward (obtain sth).
	/// Changing or adding values in here will affect GameSave and Shop. Make sure to update them accordingly.
	/// Note: Don't put "Item" in a type name, it causes problems in parsing.
	/// </summary>
	public enum ItemType
	{
		Money = 0,			// Real-life cash
		Coin = 1,			// Coins
		Gem = 2,			// Gems
		ExtraCurrency = 3,	// Extra currenty
		RemoveAds,			// Remove ads for lifetime
		RewardedVideo,		// Playing a rewarded video, for cost only
		AskFriend,          // Ask friend for item, e.g. energy, for cost only
		Character,			// A new character or skin
		CharacterExp,		// Character exp, for reward only
		CharacterLevel,		// Character level, for reward only
		Energy,				// Energy to play a level
		Inventory,			// An inventory card
		InventoryDraw,      // An inventory draw, e.g. claw machine in NH, for reward only
		Token,				// Token for a skin/item, e.g spaceship token in NH
		Weapon,				// A weapon in game, e.g. online weapon in NH
		Gadget,				// A gadget used in-game
		Consumable,			// A consumable used in-game
		Prop,				// A prop appear in-game
		World,				// A world, e.g. expansion world in NH		
		Level,				// A level in game, e.g. level in 3DTD that need to unlock before playing
		Resource,			// Resource on top of currency, e.g. resource to upgrade tech-tree in 3DTD
		Pack,				// A gift pack, normally contain a bunch of items and coins
		Revive,				// A free-revive, used for reward only
		Chest,				// A chest giving in a game, normally contain a bunch of items and coins
		Pet,				// A pet
		PetExtra,			// Secondary pet, e.g. ink pet in life rush
		Skin,				// A skin for a character
		Subscription = 100,	// Subscription
		None = 999,
	}

}
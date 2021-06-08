﻿namespace HackF5.UnitySpy.HearthstoneLib.Detail.Collection
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.Annotations;

    internal static class CollectionCardReader
    {
        public static IReadOnlyList<ICollectionCard> ReadCollection([NotNull] HearthstoneImage image)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            //try
            //{
            //    var tmp = image["CollectionManager"]?["s_instance"]?["m_collectibleCards"];
            //}
            //catch (Exception e)
            //{
            //    return null;
            //}

            var collectionCards = new Dictionary<string, CollectionCard>();
            var collectibleCards = image["CollectionManager"]?["s_instance"]?["m_collectibleCards"];

            if (collectibleCards == null)
            {
                return collectionCards.Values.ToArray();
            }

            var items = collectibleCards["_items"];
            int size = collectibleCards["_size"];
            for (var index = 0; index < size; index++)
            {
                string cardId = items[index]["m_EntityDef"]["m_cardIdInternal"];
                if (string.IsNullOrEmpty(cardId))
                {
                    continue;
                }

                int count = items[index]["<OwnedCount>k__BackingField"];
                int premium = items[index]["m_PremiumType"];

                if (!collectionCards.TryGetValue(cardId, out var card))
                {
                    card = new CollectionCard { CardId = cardId };
                    collectionCards.Add(cardId, card);
                }

                if (premium == 1)
                {
                    card.PremiumCount = count;
                }
                else if (premium == 2)
                {
                    card.DiamondCount = count;
                }
                else
                {
                    card.Count = count;
                }
            }

            return collectionCards.Values.ToArray();
        }

        public static int ReadCollectionSize([NotNull] HearthstoneImage image)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            var collectibleCards = image["CollectionManager"]["s_instance"]["m_collectibleCards"];
            var items = collectibleCards["_items"];
            int size = collectibleCards["_size"];
            var totalCards = 0;
            for (var index = 0; index < size; index++)
            {
                string cardId = items[index]["m_EntityDef"]["m_cardIdInternal"];
                if (string.IsNullOrEmpty(cardId))
                {
                    continue;
                }

                int count = items[index]["<OwnedCount>k__BackingField"] ?? 0;
                int premium = items[index]["m_PremiumType"] ?? 0;
                totalCards += count;
                totalCards += premium;
            }
            return totalCards;
        }
    }
}
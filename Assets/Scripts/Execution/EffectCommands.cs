using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EffectCommands
{
    public static IEffectCommand Get(Effect effect) => effect switch
    {
        Effect.PlacePopulationCardFromPlayer => new PlacePopulationCardFromPlayer(),
        Effect.PlaceInitialCards => new PlaceInitialCards(),
        Effect.GrowCarnivoreEcosystem => new GrowCarnivoreCommand(),
        Effect.GrowHerbivoreEcosystem => new GrowHerbivoreCommand(),
        Effect.KillCarnivoreEcosystem => new KillCarnivoreCommand(),
        Effect.KillHerbivoreEcosystem => new KillHerbivoreCommand(),
        Effect.GrowMushroomEcosystem => new GrowMushroomEcosystem(),
        Effect.Draw2 => new DrawCards(),
        Effect.Draw5 => new DrawCards(),
        Effect.OverviewSwitch => new OverviewSwitch(),
        Effect.Discard => new Discard(),
        Effect.GrowMushroom => new GrowMushroom(),
        Effect.GrowMushroomEndOfAction => new GrowMushroomEOA(),
        Effect.GrowMacrofungi => new GrowMacrofungi(),
        Effect.PlaceConstruction => new PlaceConstruction(),
        Effect.PlayAndDiscardInfluenceCard => new PlayAndDiscardInfluenceCard(),
        Effect.MovePopulationToEmptySlot => new MovePopulationToEmptySlot(),
        Effect.PlaceInfluenceOnPopulation => new PlaceInfluenceOnPopulation(),
        Effect.GiveRabies => new GiveRabies(),
        Effect.DestroyAllInTerritory => new DestroyAllInTerritory(),
        Effect.DestroyNonFungiInTerritory => new DestroyNonFungiInTerritory(),
        Effect.MakeOmnivore => new MakeOmnivore(),
        Effect.GrowPlant => new GrowPlant(),
        Effect.GrowPlantEndOfAction => new GrowPlantEOA(),
        Effect.KillPlantEndOfAction => new KillPlantEOA(),
        Effect.ObserveSeededFruit => new ObserveSeededFruit(),
        Effect.ObserveDeepRoots => new ObserveDeepRoots(),
        Effect.ObserveGreenIvy => new ObserveGreenIvy(),
        Effect.ObserveMushroomPredator => new ObserveMushroomPredator(),
        
        _ => throw new ArgumentOutOfRangeException()
    };

    public class PlacePopulationCardFromPlayer : IEffectCommand
    {
        public void Execute(PlayerAction action, Action callback)
        {
            var card = action.ActionItem as ACard;
            var owner = action.Actor;
            var slotIndex = action.Receivers[0].Index;
            ServiceLocator.Get<IModel>().RemoveCardFromHand(owner, card);
            ServiceLocator.Get<IModel>().PlaceCardOnSlot(card, owner, slotIndex);
            var location = new IView.CardLocation { Owner = owner, SlotIndex = slotIndex };
            ServiceLocator.Get<IView>().PlayCardOnSlotFromPlayer(card, owner, location, callback);
        }
    }

    public class GrowCarnivoreCommand : IEffectCommand
    {
        public void Execute(PlayerAction action, Action callback)
        {
            ServiceLocator.Get<IModel>().GrowLastPlacedPopulation(Population.Carnivore, out var parent, out _);
            var location = new IView.CardLocation
                { Owner = parent.Slot.Territory.Owner, SlotIndex = parent.Slot.SlotIndexInTerritory };
            ServiceLocator.Get<IView>().GrowPopulationCardEcosystem(location, callback);
        }
    }

    public class GrowHerbivoreCommand : IEffectCommand
    {
        public void Execute(PlayerAction action, Action callback)
        {
            ServiceLocator.Get<IModel>().GrowLastPlacedPopulation(Population.Herbivore, out var parent, out _);
            var location = new IView.CardLocation
                { Owner = parent.Slot.Territory.Owner, SlotIndex = parent.Slot.SlotIndexInTerritory };
            ServiceLocator.Get<IView>().GrowPopulationCardEcosystem(location, callback);
        }
    }

    public class KillCarnivoreCommand : IEffectCommand
    {
        public void Execute(PlayerAction action, Action callback)
        {
            var killed = ServiceLocator.Get<IModel>().KillLastPlacedPopulation(Population.Carnivore);
            var location = new IView.CardLocation
                { Owner = killed.Slot.Territory.Owner, SlotIndex = killed.Slot.SlotIndexInTerritory };
            ServiceLocator.Get<IView>().KillPopulationCardEcosystem(location, callback);
        }
    }

    public class KillHerbivoreCommand : IEffectCommand
    {
        public void Execute(PlayerAction action, Action callback)
        {
            var killed = ServiceLocator.Get<IModel>().KillLastPlacedPopulation(Population.Herbivore);
            var location = new IView.CardLocation
                { Owner = killed.Slot.Territory.Owner, SlotIndex = killed.Slot.SlotIndexInTerritory };
            ServiceLocator.Get<IView>().KillPopulationCardEcosystem(location, callback);
        }
    }

    public class Discard : IEffectCommand
    {
        public void Execute(PlayerAction action, Action callback)
        {
            ServiceLocator.Get<IModel>().RemoveCardFromHand(action.Actor, action.ActionItem as ACard);
            ServiceLocator.Get<IView>().Discard(action.Actor, callback);
        }
    }

    public class DrawCards : IEffectCommand
    {
        public void Execute(PlayerAction action, Action callback)
        {
            Dictionary<PlayerCharacter, IReadOnlyList<ACard>> cardsDrawn = new();
            foreach (var character in ServiceLocator.Get<IModel>().Config.TurnOrder)
            {
                if (character is PlayerCharacter.None) continue;
                var cards = ServiceLocator.Get<IModel>().PlayerDrawCards(character);
                cardsDrawn.Add(character, new List<ACard>(cards));
            }

            ServiceLocator.Get<IView>().DrawCards(cardsDrawn, callback);
        }
    }

    public class OverviewSwitch : IEffectCommand
    {
        public void Execute(PlayerAction action, Action callback)
        {
            ServiceLocator.Get<IView>().SwitchCamToOverview(callback);
        }
    }

    public class GrowMushroomEcosystem : IEffectCommand
    {
        public void Execute(PlayerAction action, Action callback)
        {
            var mushroom = ServiceLocator.Get<IModel>().GrowMushroomOverLastDeadPopulation();
            var slotOwner = mushroom.Slot.Territory.Owner;
            var slotIndex = mushroom.Slot.SlotIndexInTerritory;
            ServiceLocator.Get<IView>().GrowMushroom(PlayerCharacter.None,
                new IView.CardLocation { Owner = slotOwner, SlotIndex = slotIndex }, callback);
        }
    }

    public class GrowMushroom : IEffectCommand
    {
        public void Execute(PlayerAction action, Action callback)
        {
            var slotOwner = action.Receivers[0].LocationOwner;
            var slotIndex = action.Receivers[0].Index;
            ServiceLocator.Get<IModel>().GrowMushroom(slotOwner, slotIndex);
            ServiceLocator.Get<IView>().GrowMushroom(action.Actor,
                new IView.CardLocation { Owner = slotOwner, SlotIndex = slotIndex }, callback);
        }
    }

    public class GrowMushroomEOA : IEffectCommand
    {
        public void Execute(PlayerAction action, Action callback)
        {
            var slotOwner = action.Receivers[0].LocationOwner;
            var slotIndex = action.Receivers[0].Index;
            ServiceLocator.Get<IModel>().GrowMushroom(slotOwner, slotIndex);
            ServiceLocator.Get<IView>().GrowMushroom(action.Actor,
                new IView.CardLocation { Owner = slotOwner, SlotIndex = slotIndex }, callback, true);
        }
    }

    public class PlaceInitialCards : IEffectCommand
    {
        public void Execute(PlayerAction action, Action callback)
        {
            var config = ServiceLocator.Get<IModel>().Config;
            var initialCards = new Stack<PopulationCard>(config.InitialCards);
            var players = config.TurnOrder.ToList();
            players.Remove(PlayerCharacter.None);

            var cardsAndLocations = new List<(ACard card, IView.CardLocation location)>();

            int count = Mathf.Min(initialCards.Count, players.Count);
            while (count > 0)
            {
                count--;
                var index = UnityEngine.Random.Range(0, players.Count);
                var slotOwner = players[index];
                players.RemoveAt(index);

                var initialCard = initialCards.Pop();
                int slotIndex = 0;
                ServiceLocator.Get<IModel>().PlaceCardOnSlot(initialCard, slotOwner, slotIndex);
                var location = new IView.CardLocation { Owner = slotOwner, SlotIndex = slotIndex };

                cardsAndLocations.Add((initialCard, location));
            }

            ServiceLocator.Get<IView>().PlaceInitialCards(cardsAndLocations, callback);
        }
    }

    public class GrowMacrofungi : IEffectCommand
    {
        public void Execute(PlayerAction action, Action callback)
        {
            var locations = new List<IView.CardLocation>();
            PlayerCharacter slotOwner = default;
            int slotIndex = default;

            foreach (var receiver in action.Receivers)
            {
                slotOwner = receiver.LocationOwner;
                slotIndex = receiver.Index;
                locations.Add(new IView.CardLocation { Owner = slotOwner, SlotIndex = slotIndex });
                var cardIndex = receiver.SecondIndex;
                ServiceLocator.Get<IModel>().RemoveCardFromSlot(slotOwner, slotIndex, cardIndex);
            }

            var macrofungiCard = ServiceLocator.Get<IModel>().Config.Macrofungi;
            ServiceLocator.Get<IModel>().PlaceCardOnSlot(macrofungiCard, slotOwner, slotIndex, true);
            ServiceLocator.Get<IView>().GrowMacrofungi(locations.ToArray(), callback);
        }
    }

    public class PlaceConstruction : IEffectCommand
    {
        public void Execute(PlayerAction action, Action callback)
        {
            var owner = action.Receivers[0].LocationOwner;
            ServiceLocator.Get<IModel>().PlaceConstruction(owner, out var plant1, out var plant2);

            var location1 = new IView.CardLocation
            {
                SlotIndex = plant1.Slot.SlotIndexInTerritory, CardIndex = plant1.IndexInSlot,
                Owner = plant1.Slot.Territory.Owner
            };

            var location2 = new IView.CardLocation
            {
                SlotIndex = plant2.Slot.SlotIndexInTerritory, CardIndex = plant2.IndexInSlot,
                Owner = plant2.Slot.Territory.Owner
            };

            ServiceLocator.Get<IView>().PlaceConstruction(location1, location2, callback);
        }
    }

    public class PlayAndDiscardInfluenceCard : IEffectCommand
    {
        public void Execute(PlayerAction action, Action callback)
        {
            ServiceLocator.Get<IModel>().RemoveCardFromHand(action.Actor, action.ActionItem as ACard);

            bool isTerritory = action.Receivers[0].Location is ValidDropLocation.AnyTerritory;
            var location = new IView.CardLocation
            {
                IsTerritory = isTerritory,
                Owner = action.Receivers[0].LocationOwner,
                SlotIndex = isTerritory ? -1 : action.Receivers[0].Index,
                CardIndex = isTerritory ? -1 : action.Receivers[0].SecondIndex,
            };
            var influence = action.ActionItem as AInfluenceCard;
            ServiceLocator.Get<IView>().PlayAndDiscardInfluenceCard(action.Actor, influence, location, callback);
        }
    }

    public class MovePopulationToEmptySlot : IEffectCommand
    {
        public void Execute(PlayerAction action, Action callback)
        {
            var slotOwner = action.Receivers[0].LocationOwner;
            var slotIndex = action.Receivers[0].Index;
            var cardIndex = action.Receivers[0].SecondIndex;
            var targetSlotOwner = action.Receivers[1].LocationOwner;
            var targetSlotIndex = action.Receivers[1].Index;
            ServiceLocator.Get<IModel>()
                .MoveCardBetweenSlots(slotOwner, slotIndex, cardIndex, targetSlotOwner, targetSlotIndex);

            var from = new IView.CardLocation
            {
                Owner = slotOwner,
                SlotIndex = slotIndex,
                CardIndex = cardIndex
            };

            var to = new IView.CardLocation
            {
                Owner = targetSlotOwner,
                SlotIndex = targetSlotIndex
            };

            ServiceLocator.Get<IView>().MovePopulationToEmptySlot(action.Actor, from, to, callback);
        }
    }

    public class PlaceInfluenceOnPopulation : IEffectCommand
    {
        public void Execute(PlayerAction action, Action callback)
        {
            var slotOwner = action.Receivers[0].LocationOwner;
            var slotIndex = action.Receivers[0].Index;
            var cardIndex = action.Receivers[0].SecondIndex;
            var influenceCard = action.ActionItem as AInfluenceCard;
            ServiceLocator.Get<IModel>().RemoveCardFromHand(action.Actor, action.ActionItem as ACard);
            ServiceLocator.Get<IModel>().PlaceInlfuenceCardOnCard(influenceCard, slotOwner, slotIndex, cardIndex);

            var location = new IView.CardLocation
            {
                Owner = slotOwner,
                SlotIndex = slotIndex,
                CardIndex = cardIndex
            };

            ServiceLocator.Get<IView>().PlaceInfluenceOnPopulation(action.Actor, influenceCard, location, callback);
        }
    }

    public class GiveRabies : IEffectCommand
    {
        public void Execute(PlayerAction action, Action callback)
        {
            var slotOwner = action.Receivers[0].LocationOwner;
            var slotIndex = action.Receivers[0].Index;
            var cardIndex = action.Receivers[0].SecondIndex;

            ServiceLocator.Get<IModel>().GiveRabies(slotOwner, slotIndex, cardIndex);

            var location = new IView.CardLocation
            {
                Owner = slotOwner,
                SlotIndex = slotIndex,
                CardIndex = cardIndex
            };

            ServiceLocator.Get<IView>().GiveRabies(action.Actor, location, callback);
        }
    }

    public class DestroyAllInTerritory : IEffectCommand
    {
        public void Execute(PlayerAction action, Action callback)
        {
            DestroyInTerritory(action, callback, null);
        }
    }

    public class DestroyNonFungiInTerritory : IEffectCommand
    {
        public void Execute(PlayerAction action, Action callback)
        {
            DestroyInTerritory(action, callback, card => card is MushroomCard or MacrofungiCard);
        }
    }

    public static void DestroyInTerritory(PlayerAction action, Action callback, Predicate<ACard> filter)
    {
        var owner = action.Receivers[0].LocationOwner;
        Predicate<TableCard> modelFilter = filter is null ? null : tCard => filter(tCard.Card);
        ServiceLocator.Get<IModel>().RemoveCardsFromTerritory(owner, modelFilter);

        if (ServiceLocator.Get<IModel>().GetPlayer(owner).Territory.HasConstruction)
            ServiceLocator.Get<IModel>().RemoveConstruction(owner);

        ServiceLocator.Get<IView>().DestroyInTerritory(action.Actor, owner, callback, filter);
    }

    public class MakeOmnivore : IEffectCommand
    {
        public void Execute(PlayerAction action, Action callback)
        {
            var slotOwner = action.Receivers[0].LocationOwner;
            var slotIndex = action.Receivers[0].Index;
            var cardIndex = action.Receivers[0].SecondIndex;

            ServiceLocator.Get<IModel>().MakeOmnivore(slotOwner, slotIndex, cardIndex);

            var location = new IView.CardLocation
            {
                Owner = slotOwner,
                SlotIndex = slotIndex,
                CardIndex = cardIndex
            };

            ServiceLocator.Get<IView>().MakeOmnivore(action.Actor, location, callback);
        }
    }

    public class GrowPlant : IEffectCommand
    {
        public void Execute(PlayerAction action, Action callback)
        {
            var card = action.ActionItem as ACard;
            var slotOwner = action.Receivers[0].LocationOwner;
            var slotIndex = action.Receivers[0].Index;
            ServiceLocator.Get<IModel>().PlaceCardOnSlot(card, slotOwner, slotIndex);

            var location = new IView.CardLocation
            {
                Owner = slotOwner,
                SlotIndex = slotIndex,
            };

            ServiceLocator.Get<IView>().GrowPopulation(location, Population.Plant, callback);
        }
    }

    public class GrowPlantEOA : IEffectCommand
    {
        public void Execute(PlayerAction action, Action callback)
        {
            var card = ServiceLocator.Get<IModel>().Config.GetPopulationCard(Population.Plant);
            var slotOwner = action.Receivers[0].LocationOwner;
            var slotIndex = action.Receivers[0].Index;
            ServiceLocator.Get<IModel>().PlaceCardOnSlot(card, slotOwner, slotIndex);

            var location = new IView.CardLocation
            {
                Owner = slotOwner,
                SlotIndex = slotIndex,
            };

            ServiceLocator.Get<IView>().GrowPopulation(location, Population.Plant, callback, true);
        }
    }

    public class KillPlantEOA : IEffectCommand
    {
        public void Execute(PlayerAction action, Action callback)
        {
            var slotOwner = action.Receivers[0].LocationOwner;
            var slotIndex = action.Receivers[0].Index;
            var cardIndex = action.Receivers[0].SecondIndex;

            ServiceLocator.Get<IModel>().RemoveCardFromSlot(slotOwner, slotIndex, cardIndex);
            var location = new IView.CardLocation
            {
                Owner = slotOwner,
                SlotIndex = slotIndex,
                CardIndex = cardIndex
            };
            ServiceLocator.Get<IView>().KillPlacedCard(location, callback/*, true*/);
        }
    }

    public class ObserveSeededFruit : IEffectCommand
    {
        private TableCard _tableCardWherePlaced;
        private Territory _territory;
        public void Execute(PlayerAction action, Action callback)
        {
            var slotOwner = action.Receivers[0].LocationOwner;
            var slotIndex = action.Receivers[0].Index;
            var cardIndex = action.Receivers[0].SecondIndex;
            _territory = ServiceLocator.Get<IModel>().GetPlayer(slotOwner).Territory;
            _tableCardWherePlaced = _territory.Slots[slotIndex].PlacedCards[cardIndex];

            ServiceLocator.Get<IModel>().OnPopulationGrow += OnPopulationGrow;
            callback?.Invoke();
        }

        private void OnPopulationGrow(TableCard parent, TableCard child)
        {
            if (parent.Slot.Territory != _territory) return;
            
            ServiceLocator.Get<IModel>().OnPopulationGrow -= OnPopulationGrow;
            
            var growPlant = new DelayedGrowPlant(_tableCardWherePlaced.Card, _tableCardWherePlaced.Slot);
            var discard = new DelayedDiscardPlayedInfluence(_tableCardWherePlaced);
            ServiceLocator.Get<IExecutor>().PushDelayedCommand(growPlant);
            ServiceLocator.Get<IExecutor>().PushDelayedCommand(discard);
        }
    }
    
    public class ObserveDeepRoots : IEffectCommand
    {
        private TableCard _tableCardWherePlaced;
        public void Execute(PlayerAction action, Action callback)
        {
            var slotOwner = action.Receivers[0].LocationOwner;
            var slotIndex = action.Receivers[0].Index;
            var cardIndex = action.Receivers[0].SecondIndex;
            var territory = ServiceLocator.Get<IModel>().GetPlayer(slotOwner).Territory;
            _tableCardWherePlaced = territory.Slots[slotIndex].PlacedCards[cardIndex];

            _tableCardWherePlaced.OnSlotRemove += OnCardRemoved;
            callback?.Invoke();
        }

        private void OnCardRemoved()
        {
            _tableCardWherePlaced.OnSlotRemove -= OnCardRemoved;
            var growPlant = new DelayedGrowPlant(_tableCardWherePlaced.Card, _tableCardWherePlaced.Slot);
            ServiceLocator.Get<IExecutor>().PushDelayedCommand(growPlant);
        }
    }
    
    
    public class ObserveGreenIvy : IEffectCommand, IRoundEndObserver
    {
        private TableCard _tableCardWherePlaced;
        private Territory _territory;
        public void Execute(PlayerAction action, Action callback)
        {
            var slotOwner = action.Receivers[0].LocationOwner;
            var slotIndex = action.Receivers[0].Index;
            var cardIndex = action.Receivers[0].SecondIndex;
            _territory = ServiceLocator.Get<IModel>().GetPlayer(slotOwner).Territory;
            _tableCardWherePlaced = _territory.Slots[slotIndex].PlacedCards[cardIndex];
            
            _tableCardWherePlaced.OnSlotRemove += OnCardRemoved;
            ServiceLocator.Get<IRulesSystem>().RegisterRoundEndObserver(this);
            
            callback?.Invoke();
        }

        private void OnCardRemoved()
        {
            Debug.Log("hiedraverde quitado");
            _tableCardWherePlaced.OnSlotRemove -= OnCardRemoved;
            ServiceLocator.Get<IRulesSystem>().RemoveRoundEndObserver(this);
        }

        public IEnumerable<IEffectCommand> GetRoundEndEffects()
        {
            if (!_territory.HasConstruction) return new IEffectCommand[] {};
            
            _tableCardWherePlaced.OnSlotRemove -= OnCardRemoved;
            ServiceLocator.Get<IRulesSystem>().RemoveRoundEndObserver(this);

            var discard = new DelayedDiscardPlayedInfluence(_tableCardWherePlaced);
            var destroyConstruction = new DelayedDestroyConstruction(_territory);
            return new IEffectCommand[] { discard, destroyConstruction };
        }
    }
    
    public class ObserveMushroomPredator : IEffectCommand, IRoundEndObserver
    {
        private TableCard _tableCardWherePlaced;
        private Territory _territory;
        public void Execute(PlayerAction action, Action callback)
        {
            var slotOwner = action.Receivers[0].LocationOwner;
            var slotIndex = action.Receivers[0].Index;
            var cardIndex = action.Receivers[0].SecondIndex;
            _territory = ServiceLocator.Get<IModel>().GetPlayer(slotOwner).Territory;
            _tableCardWherePlaced = _territory.Slots[slotIndex].PlacedCards[cardIndex];
            
            _tableCardWherePlaced.OnSlotRemove += OnCardRemoved;
            ServiceLocator.Get<IRulesSystem>().RegisterRoundEndObserver(this);
            
            callback?.Invoke();
        }
        
        private void OnCardRemoved()
        {
            Debug.Log("depresetas quitado");
            _tableCardWherePlaced.OnSlotRemove -= OnCardRemoved;
            ServiceLocator.Get<IRulesSystem>().RemoveRoundEndObserver(this);
        }

        public IEnumerable<IEffectCommand> GetRoundEndEffects()
        {
            var lastMushroom = ServiceLocator.Get<IModel>().GetLastMushroomInTerritory(_territory.Owner);
            if (lastMushroom is null) return new IEffectCommand[] {};
            
            
            //a lo mejor renta antes un efectillo visual
            var killMushroom = new DelayedKillMushroom(lastMushroom);
            return new IEffectCommand[] { killMushroom };
        }
    }


    public class DelayedDiscardPlayedInfluence : IEffectCommand
    {
        private TableCard _tableCardWherePlaced;
        public DelayedDiscardPlayedInfluence(TableCard tableCardWherePlaced)
        {
            _tableCardWherePlaced = tableCardWherePlaced;
        }
        public void Execute(PlayerAction _, Action callback)
        {
            ServiceLocator.Get<IModel>().RemoveInfluenceCardFromCard(_tableCardWherePlaced);
            var location = new IView.CardLocation()
            {
                Owner = _tableCardWherePlaced.Slot.Territory.Owner,
                SlotIndex = _tableCardWherePlaced.Slot.SlotIndexInTerritory,
                CardIndex = _tableCardWherePlaced.IndexInSlot
            };
            ServiceLocator.Get<IView>().DiscardInfluenceFromPopulation(location, callback);
        }
    }
    
    public class DelayedGrowPlant : IEffectCommand
    {
        private ACard _card;
        private Slot _slot;
        public DelayedGrowPlant(ACard card, Slot slot)
        {
            _card = card;
            _slot = slot;
        }
        public void Execute(PlayerAction _, Action callback)
        {

            ServiceLocator.Get<IModel>().PlaceCardOnSlot(_card, _slot);

            var location = new IView.CardLocation
            {
                Owner = _slot.Territory.Owner,
                SlotIndex = _slot.SlotIndexInTerritory,
            };

            ServiceLocator.Get<IView>().GrowPopulation(location, Population.Plant, callback);
        }
    }
    
    public class DelayedDestroyConstruction : IEffectCommand
    {
        private Territory _territory;
        public DelayedDestroyConstruction(Territory territory)
        {
            _territory = territory;
        }
        public void Execute(PlayerAction _, Action callback)
        {
            ServiceLocator.Get<IModel>().RemoveConstruction(_territory.Owner);
            ServiceLocator.Get<IView>().DestroyConstruction(_territory.Owner, callback);
        }
    }
    
    public class DelayedKillMushroom : IEffectCommand
    {
        private TableCard _mushroom;
        public DelayedKillMushroom(TableCard mushroom)
        {
            _mushroom = mushroom;
        }
        public void Execute(PlayerAction _, Action callback)
        {
            ServiceLocator.Get<IModel>().RemoveCardFromSlot(_mushroom);
            var location = new IView.CardLocation
            {
                Owner = _mushroom.Slot.Territory.Owner,
                SlotIndex = _mushroom.Slot.SlotIndexInTerritory,
                CardIndex = _mushroom.IndexInSlot
            };
            ServiceLocator.Get<IView>().KillPlacedCard(location, callback);
        }
    }
}
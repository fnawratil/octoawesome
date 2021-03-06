﻿using OctoAwesome.Runtime;
using System;
using engenious;
using OctoAwesome.EntityComponents;
using OctoAwesome.Common;

namespace OctoAwesome.Client.Components
{
    internal sealed class SimulationComponent : GameComponent
    {
        private readonly IExtensionResolver extensionResolver;

        private readonly IResourceManager resourceManager;

        public Simulation Simulation { get; private set; }

        public IGameService Service { get; }

        public SimulationState State
        {
            get
            {
                if (Simulation != null)
                    return Simulation.State;
                return SimulationState.Undefined;
            }
        }

        public SimulationComponent(OctoGame game, IExtensionResolver extensionResolver, IResourceManager resourceManager) : base(game)
        {
            Service = game.Service;
            this.extensionResolver = extensionResolver;
            this.resourceManager = resourceManager;
        }

        public Guid NewGame(string name, int? seed = null)
        {
            if (Simulation != null)
            {
                Simulation.ExitGame();
                Simulation = null;
            }

            Simulation = new Simulation(resourceManager, extensionResolver, Service);
            return Simulation.NewGame(name, seed);
        }

        public void LoadGame(Guid guid)
        {
            if (Simulation != null)
            {
                Simulation.ExitGame();
                Simulation = null;
            }

            Simulation = new Simulation(resourceManager, extensionResolver, Service);
            Simulation.LoadGame(guid);
        }

        public override void Update(GameTime gameTime)
        {
            Simulation?.Update(gameTime);
        }

        public void ExitGame()
        {
            if (Simulation == null)
                return;

            Simulation.ExitGame();
            Simulation = null;
        }

        public Player LoginPlayer(string playerName)
        {
            if (Simulation == null)
                throw new NotSupportedException();

            if (Simulation.State != SimulationState.Running && Simulation.State != SimulationState.Paused)
                throw new NotSupportedException();

            //TODO: [Network] Anstelle von ID einen einstellbaren Playernamen implementieren
            Player player = resourceManager.LoadPlayer(playerName);
            player.Components.AddComponent(new RenderComponent { Name = "Wauzi", ModelName = "dog", TextureName = "texdog", BaseZRotation = -90 }, true);
            Simulation.AddEntity(player);


            return player;
        }

        public void LogoutPlayer(Player player)
        {
            if (Simulation == null)
                throw new NotSupportedException();

            if (Simulation.State != SimulationState.Running && Simulation.State != SimulationState.Paused)
                throw new NotSupportedException();

            Simulation.RemoveEntity(player);
        }
    }
}

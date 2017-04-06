namespace Engine.Systems.Activation
{
	public interface IActivationExtension : ISystemExtension
	{
		void OnNotActive(int itemId, Components.Activation activation);

		void OnActivating(int itemId, Components.Activation activation);

		void OnActive(int itemId, Components.Activation activation);

		void OnDeactivating(int itemId, Components.Activation activation);
	}
}

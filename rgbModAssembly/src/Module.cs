using Assets.Scripts.Missions;

namespace rgbMod
{
	public class Module
	{
		public BombComponent BombComponent { get; private set; }
		public int BombId { get; private set; }
		public string ModuleName;
		public Selectable selectable;
		public bool IsKeyModule = false;

		private bool interacting = false;

		public ComponentTypeEnum ComponentType;

		public bool IsSolved => BombComponent.IsSolved;

		public bool IsSolvable => BombComponent.IsSolvable;

		public Module(BombComponent bombComponent, int bombId)
		{
			BombComponent = bombComponent;
			BombId = bombId;
			if (bombComponent.ComponentType != ComponentTypeEnum.Empty && BombComponent.ComponentType != ComponentTypeEnum.Timer)
			{
				selectable = bombComponent.GetComponent<Selectable>();
			}
		}

		public void Update()
		{
			if (selectable != null)
			{
				if (KTInputManager.Instance?.SelectableManager?.GetCurrentParent()?.GetComponentInParent<BombComponent>() != null)
				{
					if (KTInputManager.Instance.SelectableManager.GetCurrentParent().GetComponentInParent<BombComponent>() == BombComponent)
						interacting = true;
					else
						interacting = false;
				}
				else
					interacting = false;
			}
		}

		public bool IsHeld()
		{
			return interacting;
		}
	}
}
using Microsoft.Xna.Framework;
using Multiplayer_Games_Programming_Framework.Core;

namespace Multiplayer_Games_Programming_Framework.GameCode.Components
{
	internal class PaddleNetworkController : Component
	{
		int m_Index;
		Rigidbody m_Rigidbody;

		public PaddleNetworkController(GameObject gameObject, int index) : base(gameObject)
		{
			m_Index = index;
		}

		protected override void Start(float deltaTime)
		{
			m_Rigidbody = m_GameObject.GetComponent<Rigidbody>();
		}

		public void UpdatePosition(Vector2 pos)
		{
			m_Rigidbody.UpdatePosition(pos);
		}
	}
}

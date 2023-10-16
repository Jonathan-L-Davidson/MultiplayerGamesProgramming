
namespace Multiplayer_Games_Programming_Framework
{
	internal class DestoryInTime : Component
	{
		float m_Timer;
		public DestoryInTime(GameObject gameObject, float time) : base(gameObject)
		{
			m_Timer = time;
		}

		override protected void Update(float deltaTime)
		{
			m_Timer -= deltaTime;
			if (m_Timer <= 0)
			{
				m_GameObject.Destroy();
			}
		}
	}
}

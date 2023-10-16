using System;
using System.Reflection;
using nkast.Aether.Physics2D.Dynamics.Contacts;
using nkast.Aether.Physics2D.Dynamics;

namespace Multiplayer_Games_Programming_Framework
{
    internal abstract class Component
    {
        public GameObject m_GameObject { get; private set; }
        public Transform m_Transform { get { return m_GameObject.m_Transform; } }

        public bool m_Enabled = true;

        public int m_ExecutionOrder = 0;

        public Component(GameObject gameObject)
        {
            m_GameObject = gameObject;
        }


        public virtual void Destroy()
        {
            {
                m_GameObject.RemoveComponent(this);
                m_GameObject = null;
            }
        }

		#region Game Loop

		protected virtual void Start(float deltaTime) { }

		protected virtual void Draw(float deltaTime) { }

		protected virtual void Update(float deltaTime) { }

		protected virtual void LateUpdate(float deltaTime) { }

		protected virtual void ComponentAdded(Component component) { }

        protected virtual void OnCollisionEnter(Fixture sender, Fixture other, Contact contact) { }
        protected virtual void OnCollisionExit(Fixture sender, Fixture other, Contact contact) { }

		#endregion

		#region Reflection Checks

		public Action<float> CheckIfUsingGameLoopMethod(string methodName)
        {
			Type type = GetType();

            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            MethodInfo[] method = type.GetMethods(flags);

            for(int i = 0; i < method.Length; ++i)
            {
                if (method[i].DeclaringType.Name == type.Name && method[i].Name == methodName)
                {
				    switch (methodName)
				    {
					    case "Start":
						    return Start;
					    case "Draw":
						    return Draw;
					    case "Update":
						    return Update;
					    case "LateUpdate":
						    return LateUpdate;
				    }
				}
			}
            return null;
		}

		public Action<Component> CheckIfUsingComponentAddedMethod(string methodName)
		{
			Type type = GetType();

			BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
			MethodInfo[] method = type.GetMethods(flags);

			for (int i = 0; i < method.Length; ++i)
			{
				if (method[i].DeclaringType != typeof(Component) && method[i].Name == methodName)
				{
					switch (methodName)
					{
						case "ComponentAdded":
							return ComponentAdded;
					}
				}
			}
			return null;
		}

		public Action<Fixture, Fixture, Contact> CheckIfUsingCollisionMethods(string methodName)
		{
			Type type = GetType();

			BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
			MethodInfo[] method = type.GetMethods(flags);

			for (int i = 0; i < method.Length; ++i)
			{
				if (method[i].DeclaringType.Name == type.Name && method[i].Name == methodName)
				{
					switch (methodName)
					{
						case "OnCollisionEnter":
							return OnCollisionEnter;
						case "OnCollisionExit":
							return OnCollisionExit;
					}
				}
			}
			return null;
		}

	#endregion
	}
}

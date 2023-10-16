using System;
using System.Collections.Generic;
using System.Diagnostics;
using nkast.Aether.Physics2D.Dynamics.Contacts;
using nkast.Aether.Physics2D.Dynamics;

namespace Multiplayer_Games_Programming_Framework
{
    internal class GameObject
    {
        public string m_Name;
        public Scene m_Scene { get; private set; }
        public Transform m_Transform;

        List<Component> m_GameComponents;

		event Action<float> OnStartCalls;
		event Action<float> OnDrawCalls;
		event Action<float> OnUpdateCalls;
		event Action<float> OnLateUpdateCalls;

        event Action<Component> OnComponentAdded;

        event Action<Fixture, Fixture, Contact> OnCollisionEnter;
        event Action<Fixture, Fixture, Contact> OnCollisionExit;

		public GameObject(Scene scene, Transform transform)
        {
            m_Name = "GameObject " + Guid.NewGuid();
            m_Scene = scene;
            m_Transform = transform;
            m_GameComponents = new List<Component>();
		}

        ~GameObject()
        {
			Console.WriteLine(m_Name + " destroyed");
        }

        /// <summary>
        /// Creates an instance of a GameObject and adds it to the scenes object list
        /// </summary>
        /// <typeparam name="T">GameObject Type</typeparam>
        /// <param name="scene">Reference to the scene this object is located in</param>
        /// <param name="transform">Position, Rotation and Scale of object</param>
        /// <param name="components">Components to be added on Creation</param>
        /// <returns>GameObject reference</returns>
		public static T Instantiate<T>(Scene scene, Transform transform) where T : GameObject
        {
            GameObject go = Activator.CreateInstance(typeof(T), scene, transform) as GameObject;
            go.LoadContent();
            scene.AddGameObject(go);
            return go as T;
        }

        /// <summary>
        /// Adds a component to a GameObject Reference
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gameObject"></param>
        /// <param name="component"></param>
        /// <returns></returns>
        public T AddComponent<T>(T component) where T : Component
        {
            m_GameComponents.Add(component as Component);

            string[] methodNames = { "Start", "Draw", "Update", "LateUpdate"};

            for(int i = 0; i < methodNames.Length; i++)
            {
			    Action<float> method = component.CheckIfUsingGameLoopMethod(methodNames[i]);
				if (method != null)
				{
					RegisterComponentMethods(component, method);
				}
			}

			OnComponentAdded?.Invoke(component);

            Action<Component> comMethod = component.CheckIfUsingComponentAddedMethod("ComponentAdded");
            if(comMethod != null)
            {
				OnComponentAdded += comMethod;
			}

			string[] colMethodNames = { "OnCollisionEnter", "OnCollisionExit"};

            for (int i = 0; i < colMethodNames.Length; i++)
            {
                Action<Fixture, Fixture, Contact> colMethod = component.CheckIfUsingCollisionMethods(colMethodNames[i]);
                if (colMethod != null)
                {
                    switch (colMethodNames[i])
                    {
						case "OnCollisionEnter":
							OnCollisionEnter += colMethod;
							break;
						case "OnCollisionExit":
							OnCollisionExit += colMethod;
							break;
					}
                }
            }

            return component;
        }

		/// <summary>
		/// Returns the first component of type T
		/// </summary>
		/// <typeparam name="T">Component Type</typeparam>
		/// <returns>Returns the first component of type T</returns>
		public T GetComponent<T>() where T : Component
        {
            foreach (Component component in m_GameComponents)
            {
                if (component is T)
                {
                    return component as T;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a list of components of type T
        /// </summary>
        /// <typeparam name="T">Component type</typeparam>
        /// <returns>Returns a list of components of type T</returns>
        public List<T> GetComponents<T>() where T : Component
        {
            List<T> components = new List<T>();

            foreach (Component component in m_GameComponents)
            {
                if (component is T)
                {
                    components.Add(component as T);
                }
            }

            return components;
        }

        public void RemoveComponent(Component component)
        {
            bool removed = m_GameComponents.Remove(component);
            if (removed)
            {
                DeregisterGameLoopCall(component);
				DeregisterCollisionCall(component);
				DeregisterOnComponentAddedCall(component);
			    component.Destroy();
            }
		}

		/// <summary>
		/// Destroys the GameObject
		/// </summary>
		public void Destroy()
        {
			m_Scene.RemoveGameObject(this);

            for (int i = m_GameComponents.Count - 1; i >= 0 ; --i)
            {
                RemoveComponent(m_GameComponents[i]);
            }

            m_GameComponents.Clear();
            m_Transform = null;
        }

		#region GameLoop methods

        /// <summary>
        /// Called once the object has been instantiated but before added to the scene
        /// </summary>
		protected virtual void LoadContent() { }

		private void Start(float deltaTime)
		{
			OnStartCalls?.Invoke(deltaTime);
			OnStartCalls = null;
		}

		private void Draw(float deltaTime)
		{
			OnDrawCalls?.Invoke(deltaTime);
		}

		private void Update(float deltaTime)
		{
			OnUpdateCalls?.Invoke(deltaTime);
		}

		private void LateUpdate(float deltaTime)
        {
			OnLateUpdateCalls?.Invoke(deltaTime);
		}

		#endregion

		#region Register methods

		private void RegisterComponentMethods(Component component, Action<float> method)
		{
			switch (method.Method.Name)
			{
				case "Start":
					OnStartCalls += method;

                    if(OnStartCalls.GetInvocationList().Length == 1)
                    {
						m_Scene.RegisterGameLoopCall(Start);
					}
					break;
				case "Draw":
					OnDrawCalls += method;

					if (OnDrawCalls.GetInvocationList().Length == 1)
					{
						m_Scene.RegisterGameLoopCall(Draw);
					}
					break;
				case "Update":
					OnUpdateCalls += method;

					if (OnUpdateCalls.GetInvocationList().Length == 1)
					{
						m_Scene.RegisterGameLoopCall(Update);
					}
					break;
				case "LateUpdate":
					OnLateUpdateCalls += method;

					if (OnLateUpdateCalls.GetInvocationList().Length == 1)
					{
                        m_Scene.RegisterGameLoopCall(LateUpdate);
                    }
					break;
				default:
					Debug.Fail("Invalid funcation call");
					break;
			}
		}

        public void DeregisterGameLoopCall(Component component)
        {
			(bool, string) UnsubscribeFromEvent (ref Action<float> eventListener)
			{
				Delegate[] del = eventListener?.GetInvocationList();
				bool hadCall = false;
				string methodName = "";

				if (del != null)
				{
					foreach (Delegate d in del)
					{
						if (d.Target == component)
						{
							eventListener -= (Action<float>)d;
							hadCall = true;
							methodName = d.Method.Name;
							break;
						}
					}
				}

				return (hadCall, methodName);
			}

			void RemoveCallFromScene(string methodName)
			{
				switch (methodName)
				{
					case "Start":
						if (OnStartCalls?.GetInvocationList().Length == 0)
						{
							m_Scene.DeregisterGameLoopCall(Start);
						}
						break;
					case "Draw":
						if (OnDrawCalls?.GetInvocationList().Length == 0)
						{
							m_Scene.DeregisterGameLoopCall(Draw);
						}
						break;
					case "Update":
						if (OnUpdateCalls?.GetInvocationList().Length == 0)
						{
							m_Scene.DeregisterGameLoopCall(Update);
						}
						break;
					case "LateUpdate":
						if (OnLateUpdateCalls?.GetInvocationList().Length == 0)
						{
							m_Scene.DeregisterGameLoopCall(LateUpdate);
						}
						break;
				}
			}

			void CheckCalls((bool, string) method)
			{
				if (method.Item1)
				{
					RemoveCallFromScene(method.Item2);
				}
			}

			CheckCalls(UnsubscribeFromEvent(ref OnStartCalls));
			CheckCalls(UnsubscribeFromEvent(ref OnUpdateCalls));
			CheckCalls(UnsubscribeFromEvent(ref OnLateUpdateCalls));
			CheckCalls(UnsubscribeFromEvent(ref OnDrawCalls));
		}
		
		public void DeregisterCollisionCall(Component component)
		{
			void UnsubscribeFromEvent(ref Action<Fixture, Fixture, Contact> eventListener)
			{
				Delegate[] del = eventListener?.GetInvocationList();

				if (del != null)
				{
					foreach (Delegate d in del)
					{
						if (d.Target == component)
						{
							eventListener -= (Action<Fixture, Fixture, Contact>)d;
							break;
						}
					}
				}
			}

			UnsubscribeFromEvent(ref OnCollisionEnter);
			UnsubscribeFromEvent(ref OnCollisionExit);
		}

		public void DeregisterOnComponentAddedCall(Component component)
		{
			void UnsubscribeFromEvent(ref Action<Component> eventListener)
			{
				Delegate[] del = eventListener?.GetInvocationList();

				if (del != null)
				{
					foreach (Delegate d in del)
					{
						if (d.Target == component)
						{
							eventListener -= (Action<Component>)d;
							break;
						}
					}
				}
			}

			UnsubscribeFromEvent(ref OnComponentAdded);
		}

		#endregion

		#region Collision Methods

		public bool CollisionEnter(Fixture sender, Fixture other, Contact contact)
        {
			OnCollisionEnter?.Invoke(sender, other, contact);
			return true;
        }

		public void CollisionExit(Fixture sender, Fixture other, Contact contact)
		{
			OnCollisionExit?.Invoke(sender, other, contact);
		}

		#endregion

	}
}

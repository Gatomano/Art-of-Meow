﻿using System;
using System.Diagnostics;
using AoM;
using Cells;
using Cells.CellObjects;
using Cells.Collision;
using Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moggle;
using Moggle.Controles;
using Units.Buffs;
using Units.Equipment;
using Units.Inteligencia;
using Units.Order;
using Units.Recursos;

namespace Units
{
	/// <summary>
	/// Game player
	/// </summary>
	public class Unidad : IUnidad, IDibujable
	{
		/// <summary>
		/// Gets the name
		/// </summary>
		/// <value>The nombre.</value>
		public string Nombre { get; }

		/// <summary>
		/// Gets the team's id
		/// </summary>
		public TeamManager Team { get; set; }

		/// <summary>
		/// Desarga el contenido gráfico.
		/// </summary>
		public void UnloadContent ()
		{
		}

		/// <summary>
		/// Enqueues a primitive order 
		/// </summary>
		public void EnqueueOrder (IPrimitiveOrder order)
		{
			PrimitiveOrders.Queue (order);
		}

		bool IUpdateGridObject.IsReady { get { return PrimitiveOrders.IsIdle; } }

		/// <summary>
		/// Gets the inventory of this unit
		/// </summary>
		public Inventory Inventory { get; }

		IInventory IUnidad.Inventory { get { return Inventory; } }

		/// <summary>
		/// Gets the grid.
		/// </summary>
		/// <value>The grid.</value>
		public Grid Grid { get; set; }

		Grid IUnidad.MapGrid { get { return Grid; } }

		/// <summary>
		/// Gets the orders corresponding this unidad
		/// </summary>
		/// <value>The primitive orders.</value>
		public OrderQueue PrimitiveOrders { get; }

		IComponentContainerComponent<IControl> IControl.Container 
		{ get { return (IComponentContainerComponent<IControl>)Grid; } }

		/// <summary>
		/// Execute this unidad's turn
		/// </summary>
		public virtual void Execute ()
		{
			Inteligencia.DoAction ();
		}

		/// <summary>
		/// Pass time
		/// </summary>
		/// <param name="time">Time.</param>
		public void PassTime (float time)
		{
			PrimitiveOrders.PassTime (time);
			Update (time);
		}

		/// <summary>
		/// Initialize this instance.
		/// </summary>
		public void Initialize ()
		{
		}

		/// <summary>
		/// Gets the time for the next action.
		/// </summary>
		public float NextActionTime
		{
			get
			{
				return PrimitiveOrders.ExpectedFirstOrderTerminationTime ();
			}
		}

		/// <summary>
		/// Gets the resources of this unit
		/// </summary>
		/// <value>The recursos.</value>
		public ManejadorRecursos Recursos { get; }

		/// <summary>
		/// Gets the buffs if this unit
		/// </summary>
		/// <value>The buffs.</value>
		public BuffManager Buffs { get; }

		/// <summary>
		/// Gets the equipment of this unit
		/// </summary>
		/// <value>The equipment.</value>
		public EquipmentManager Equipment { get; }

		/// <summary>
		/// Gets the skills of this unit
		/// </summary>
		/// <value>The skills.</value>
		public Units.Skills.SkillManager Skills { get; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="Units.Unidad"/> is habilitado.
		/// </summary>
		/// <value><c>true</c> if habilitado; otherwise, <c>false</c>.</value>
		public bool Habilitado { get { return RecursoHP.Valor > 0; } }

		bool IUpdateGridObject.Enabled { get { return Habilitado; } }

		/// <summary>
		/// Gets the HP
		/// </summary>
		public RecursoHP RecursoHP { get; private set; }

		const string TextureType = "swordman";

		/// <summary>
		/// Gets or sets the texture string.
		/// </summary>
		/// <value>The texture string.</value>
		public string TextureStr { get; protected set; }

		/// <summary>
		/// Gets the texture used to draw this object
		/// </summary>
		/// <value>The texture.</value>
		public Texture2D Texture { get; protected set; }

		/// <summary>
		/// Carga el contenido gráfico de la unidad, equipment e inventory
		/// </summary>
		/// <param name="content">Content.</param>
		public void InitializeContent (BibliotecaContenido content)
		{
			Texture = content.GetContent<Texture2D> (TextureStr);
		}

		/// <summary>
		/// Add its texture to the content
		/// </summary>
		public void AddContent (BibliotecaContenido manager)
		{
			manager.AddContent (TextureStr);
		}

		/// <summary>
		/// Determina si este objeto evita que otro objeto pueda ocupar esta misma celda.
		/// </summary>
		/// <param name="collObj">Coll object.</param>
		public bool Collision (IGridObject collObj)
		{
			if (!Habilitado)
				return false;
			
			var collUnid = collObj as IUnidad;
			if (collUnid != null)
			{
				if (collUnid.Habilitado)
					return true;
			}
			return false;
		}

		float hpRelativeValue
		{
			get { return RecursoHP.RelativeHp; }
		}

		/// <summary>
		/// Dibuja el objeto sobre un rectpangulo específico
		/// </summary>
		/// <param name="bat">Batch</param>
		/// <param name="area">Rectángulo de dibujo</param>
		public void Draw (SpriteBatch bat, Rectangle area)
		{
			if (Habilitado)
				ForceDraw (area, bat);
		}

		/// <summary>
		/// Draw the unidad, even if it not enabled
		/// </summary>
		/// <param name="bat">Batch</param>
		/// <param name="area">Rectángulo de dibujo</param>
		public void ForceDraw (Rectangle area, SpriteBatch bat)
		{
			bat.Draw (
				Texture,
				area, null, Team.TeamColor,
				0, Vector2.Zero,
				SpriteEffects.None,
				Depths.Unit);


			// Barras
			const int ht = 3;
			int sepPoint = (int)(area.Width * hpRelativeValue);
			var rec = new Rectangle (
				          area.Left + sepPoint,
				          area.Bottom,
				          area.Width - sepPoint,
				          ht);

			bat.Draw (
				Juego.Textures.SolidTexture,
				destinationRectangle: rec,
				color: Color.DarkGray * 0.7f,
				layerDepth: Depths.Gui);
			var fgRect = new Rectangle (area.Left, rec.Top, sepPoint, ht);

			bat.Draw (
				Juego.Textures.SolidTexture,
				destinationRectangle: fgRect,
				color: Color.Red,
				layerDepth: Depths.Gui);
		}

		/// <summary>
		/// Releases all resource used by the <see cref="Units.Unidad"/> object.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="Units.Unidad"/>. The <see cref="Dispose"/>
		/// method leaves the <see cref="Units.Unidad"/> in an unusable state. After calling <see cref="Dispose"/>, you must
		/// release all references to the <see cref="Units.Unidad"/> so the garbage collector can reclaim the memory that the
		/// <see cref="Units.Unidad"/> was occupying.</remarks>
		public void Dispose ()
		{
			if (Grid.Objects.Contains (this))
				throw new Exception ("Cannot dispose if this unidad is still present in game.");
			Texture = null;
		}

		/// <summary>
		/// Gets the cell-based localization.
		/// </summary>
		public Point Location { get; set; }

		/// <summary>
		/// Move or melee to a direction
		/// </summary>
		/// <returns><c>true</c>, if action was taken, <c>false</c> otherwise.</returns>
		/// <param name="dir">Direction</param>
		public bool MoveOrMelee (MovementDirectionEnum dir)
		{
			var desde = Location;
			// Intenta mover este objeto; si no puede, intenta atacar.
			if (!Grid.MoveCellObject (this, dir))
			{
				// Do melee
				var targetCell = new Cell (Grid, Location + dir.AsDirectionalPoint ());
				var target = targetCell.GetUnidadHere ();
				if (target == null)
					return false;

				// Construct the order
				assertIsIdleCheck ();// Unidad debe estar idle para llegar aquí
				var melee = Equipment.GetMeleeDamageType ();
				melee.DoMeleeEffectOn (this, target);
			}
			else
			{
				assertIsIdleCheck ();// Unidad debe estar idle para llegar aquí
				PrimitiveOrders.Queue (new CooldownOrder (
					this,
					calcularTiempoMov (
						desde,
						Location)));
			}
			return true;
		}

		[Conditional ("DEBUG")]
		internal void assertIsIdleCheck ()
		{
			if (!PrimitiveOrders.IsIdle)
				throw new Exception ();
		}

		float calcularTiempoMov (Point desde, Point hasta)
		{
			var vel = Recursos.ValorRecurso (ConstantesRecursos.Velocidad);
			var cellOrig = Grid.GetCell (desde);
			var cellDest = Grid.GetCell (hasta);
			var peso = (cellOrig.PesoMovimiento () + cellDest.PesoMovimiento ()) / 2;

			if (desde.X != hasta.X && desde.Y != hasta.Y) // Mov inclinado
				peso *= 1.4f;

			Debug.WriteLine (
				string.Format (
					"Peso de movimiento de {0} a {1} por {2} en {3}",
					desde,
					hasta,
					this,
					peso),
				"Movimiento");
			return peso / vel;
		}

		/// <summary>
		/// Gets or sets the controller of this unidad
		/// </summary>
		/// <value>The inteligencia.</value>
		public IUnidadController Inteligencia { get; set; }

		/// <summary>
		/// Update
		/// </summary>
		/// <param name="gameTime">Game time.</param>
		public void Update (float gameTime)
		{
			if (Habilitado)
				ForceUpdate (gameTime);
		}

		/// <summary>
		/// Updates this unidad, even if it is not <see cref="Habilitado"/>
		/// </summary>
		/// <param name="gameTime">Game time.</param>
		protected void ForceUpdate (float gameTime)
		{
			Recursos.Update (gameTime);
			Buffs.Update (gameTime);
		}

		System.Collections.Generic.IEnumerable<ICollisionRule> ICollidableGridObject.GetCollisionRules ()
		{
			yield return new DescriptCollitionRule (z => Habilitado && ((z as IUnidad)?.Habilitado ?? false)); // Does not stack
		}

		/// <summary>
		/// Occurs when this unidad is killed.
		/// </summary>
		public event EventHandler Killed
		{
			add
			{
				RecursoHP.ReachedZero += value;
			}
			remove
			{
				RecursoHP.ReachedZero -= value;
			}
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		public override string ToString ()
		{
			return string.Format ("[Unidad: {0}", Nombre);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Units.Unidad"/> class.
		/// </summary>
		/// <param name="texture">Texture name</param>
		/// <param name="grid">Game grid</param>
		public Unidad (Grid grid, string texture = TextureType)
		{
			Grid = grid;
			Nombre = getNextName ();
			TextureStr = texture;
			Recursos = new ManejadorRecursos (this);
			PrimitiveOrders = new OrderQueue ();
			Equipment = new EquipmentManager (this);
			Buffs = new BuffManager (this);
			Inventory = new Inventory ();
			Skills = new Units.Skills.SkillManager (this);
			inicializarRecursos ();
		}

		void inicializarRecursos ()
		{
			RecursoHP = new RecursoHP (this)
			{
				Max = 5,
				Valor = 5
			};
			Recursos.Add (RecursoHP);
			Recursos.Add (new StatRecurso (ConstantesRecursos.Velocidad, this)
			{
				TasaRecuperaciónNormal = 1,
				TasaRecuperaciónMax = 0.5f,
				Base = 10,
				Max = 10,
				Valor = 10
			});
			Recursos.Add (new StatRecurso (ConstantesRecursos.Destreza, this)
			{
				TasaRecuperaciónNormal = 1,
				TasaRecuperaciónMax = 0.5f,
				Base = 7,
				Max = 7,
				Valor = 7
			});
			Recursos.Add (new StatRecurso (ConstantesRecursos.Fuerza, this)
			{
				TasaRecuperaciónNormal = 1,
				TasaRecuperaciónMax = 0.5f,
				Base = 10,
				Max = 10,
				Valor = 10
			});
		}

		static int nextId;

		static string getNextName ()
		{
			return "Unidad " + nextId++;
		}
	}
}
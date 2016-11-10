﻿using System;
using System.Collections.Generic;
using System.Linq;
using Cells.CellObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moggle.Controles;
using MonoGame.Extended;
using MonoGame.Extended.Shapes;

namespace Cells
{
	/// <summary>
	/// The Griod system
	/// </summary>
	public class GridControl : DSBC, IComponentContainerComponent<IGridObject>
	{
		/// <summary>
		/// Devuelve el tablero lógico
		/// </summary>
		public LogicGrid Grid { get; private set; }

		/// <summary>
		/// Cambia el tablero actual, liberando completamente al anterior e inicializando el nuevo
		/// </summary>
		/// <param name="newGrid">Nuevo tablero lógico</param>
		public void ChangeGrid (LogicGrid newGrid)
		{
			Grid.Dispose ();
			Grid = newGrid;
			reInitialize ();
		}

		void reInitialize ()
		{
			// TODO
			Initialize ();
			var cont = Game.Contenido;
			AddContent (cont);
			cont.Load ();
			InitializeContent (cont);
		}

		ICollection<IGridObject> _objects { get { return Grid.Objects; } }

		/// <summary>
		/// The size of a cell (Draw)
		/// </summary>
		public SizeF CellSize = new SizeF (24, 24);

		/// <summary>
		/// Celda de _data que se muestra en celda visible (0,0)
		/// </summary>
		public Point CurrentVisibleTopLeft = Point.Zero;

		/// <summary>
		/// Posición top left del control.
		/// </summary>
		public Point ControlTopLeft = Point.Zero;
		/// <summary>
		/// Gets the number of visible cells
		/// </summary>
		public Size VisibleCells = new Size (50, 20);

		/// <summary>
		/// Gets the size of this grid, as a <see cref="IControl"/>
		/// </summary>
		/// <value>The size of the control.</value>
		public Size ControlSize
		{
			get
			{
				return new Size ((int)(VisibleCells.Width * CellSize.Width),
					(int)(VisibleCells.Height * CellSize.Height));
			}
		}

		/// <summary>
		/// Devuelve la posición de un spot de celda (por lo tanto coordenadas absolutas)
		/// </summary>
		/// <param name="p">coordenadas del spot</param>
		public Point CellSpotLocation (Point p)
		{
			var _x = (int)(ControlTopLeft.X + CellSize.Width * (p.X - CurrentVisibleTopLeft.X));
			var _y = (int)(ControlTopLeft.Y + CellSize.Height * (p.Y - CurrentVisibleTopLeft.Y));
			return new Point (_x, _y);
		}

		/// <summary>
		/// Gets the bounds
		/// </summary>
		/// <value>The bounds.</value>
		public RectangleF Bounds
		{
			get
			{
				return new RectangleF (ControlTopLeft.ToVector2 (), ControlSize);
			}
		}

		/// <summary>
		/// Devuelve la posición de un spot de celda (por lo tanto coordenadas absolutas)
		/// </summary>
		/// <param name="x">Coordenada X</param>
		/// <param name="y">Coordenada Y</param>
		public Point CellSpotLocation (int x, int y)
		{
			return CellSpotLocation (new Point (x, y));
		}

		/// <summary>
		/// Dibuja el control.
		/// </summary>
		/// <param name="gameTime">Game time.</param>
		protected override void Draw (GameTime gameTime)
		{
			//var bat = Screen.
			//bat.Begin (SpriteSortMode.BackToFront);
			var bat = Screen.Batch;
			foreach (var x in _objects)
			{
				if (IsVisible (x.Location))
				{
					var rectOutput = new Rectangle (
						                 CellSpotLocation (x.Location),
						                 (Size)CellSize);
					x.Draw (bat, rectOutput);
				}
			}
		}


		/// <summary>
		/// Devuelve el límite gráfico del control.
		/// </summary>
		/// <returns>The bounds.</returns>
		protected override IShapeF GetBounds ()
		{
			return Bounds;
		}

		/// <summary>
		/// Agrega el contenido a la biblitoeca
		/// </summary>
		protected override void AddContent (Moggle.BibliotecaContenido manager)
		{
			base.AddContent (manager);
			foreach (var x in _objects)
				x.AddContent (manager);
		}

		/// <summary>
		/// Vincula el contenido a campos de clase
		/// </summary>
		/// <param name="manager">Manager.</param>
		protected override void InitializeContent (Moggle.BibliotecaContenido manager)
		{
			base.InitializeContent (manager);
			foreach (var x in _objects)
				x.InitializeContent (manager);
		}

		/// <summary>
		/// Update lógico
		/// </summary>
		/// <param name="gameTime">Game time.</param>
		public override void Update (GameTime gameTime)
		{
			Grid.TimeManager.ExecuteNext ();
		}

		/// <summary>
		/// Se ejecuta antes del ciclo, pero después de saber un poco sobre los controladores.
		/// No invoca LoadContent por lo que es seguro agregar componentes
		/// </summary>
		public override void Initialize ()
		{
			base.Initialize ();
			Grid.AddedObject += itemAdded;
		}

		/// <summary>
		/// Releases all resource used by the <see cref="Cells.GridControl"/> object.
		/// Unsusbribe to Grid's events; so it can be collected by GC.
		/// </summary>
		/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="Cells.GridControl"/>. The
		/// <see cref="Dispose"/> method leaves the <see cref="Cells.GridControl"/> in an unusable state. After calling
		/// <see cref="Dispose"/>, you must release all references to the <see cref="Cells.GridControl"/> so the garbage
		/// collector can reclaim the memory that the <see cref="Cells.GridControl"/> was occupying.</remarks>
		protected override void Dispose ()
		{
			Grid.AddedObject -= itemAdded;
			base.Dispose ();
		}

		#region Cámara

		/// <summary>
		/// Centra el campo visible en la dirección de una celda.
		/// </summary>
		/// <param name="p">P.</param>
		public void TryCenterOn (Point p)
		{
			var left = Math.Max (0, p.X - VisibleCells.Width / 2);
			var top = Math.Max (0, p.Y - VisibleCells.Height / 2);
			CurrentVisibleTopLeft = new Point (left, top);
		}

		/// <summary>
		/// Determina si una dirección de celda es visible actualmente.
		/// </summary>
		/// <param name="p">Dirección de celda.</param>
		public bool IsVisible (Point p)
		{
			return GetVisivilityBox ().Contains (p);
		}

		/// <summary>
		/// Gets a rectangle representing the edges (mod grid) of the view
		/// </summary>
		public Rectangle GetVisivilityBox ()
		{
			return new Rectangle (CurrentVisibleTopLeft, VisibleCells);
		}

		/// <summary>
		/// The size of the edge.
		/// Objects outside this area are considered as "centered enough"
		/// </summary>
		static Size _edgeSize = new Size (4, 3);

		/// <summary>
		/// Centers the view on a given object, if it is not centered enough.
		/// </summary>
		public void CenterIfNeeded (IGridObject obj)
		{
			if (obj == null)
				throw new ArgumentNullException ("obj");

			if (!IsInCenter (obj.Location, _edgeSize))
				TryCenterOn (obj.Location);
		}

		/// <summary>
		/// Determines if a given point is centered enough
		/// </summary>
		/// <returns><c>true</c> if the given point is centered enough; otherwise, <c>false</c>.</returns>
		/// <param name="p">Grid-based point</param>
		/// <param name="edge_size">Size of the "not centered" area</param>
		/// <seealso cref="CenterIfNeeded"/>
		/// <seealso cref="_edgeSize"/>
		bool IsInCenter (Point p, Size edge_size)
		{
			var edge = GetVisivilityBox ();
			edge.Inflate (-edge_size.Width, -edge_size.Height);
			return edge.Contains (p);
		}

		#endregion

		#region Component container

		void IComponentContainerComponent<IGridObject>.AddComponent (IGridObject component)
		{
			_objects.Add (component);
		}

		bool IComponentContainerComponent<IGridObject>.RemoveComponent (IGridObject component)
		{
			return _objects.Remove (component);
		}

		IEnumerable<IGridObject> IComponentContainerComponent<IGridObject>.Components
		{
			get
			{
				return _objects;
			}
		}

		#endregion

		#region CollectionObserve

		void itemAdded (object sender, IGridObject e)
		{
			if (e.Texture == null)
			{
				// cargar textura
				var cont = Game.Contenido;
				e.AddContent (cont);
				cont.Load ();
				e.InitializeContent (cont);
			}
		}

		#endregion


		/// <summary>
		/// Initializes a new instance of the <see cref="Cells.GridControl"/> class.
		/// </summary>
		/// <param name="grid">El tablero lógico</param>
		/// <param name="scr">Screen where this grid belongs to</param>
		public GridControl (LogicGrid grid, Moggle.Screens.IScreen scr)
			: base (scr)
		{
			Grid = grid;
		}
	}
}
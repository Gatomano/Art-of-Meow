﻿using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Shapes;

namespace Cells.CellObjects
{
	public class BackgroundObject : IGridObject
	{
		public readonly string StringTexture;

		public Texture2D Texture { get; private set; }

		public BackgroundObject (Point loc,
		                         string texture)
		{
			StringTexture = texture;
			Location = loc;
		}

		bool IGridObject.Collision (IGridObject collObj)
		{
			return false;
		}

		public void LoadContent (ContentManager content)
		{
			Texture = content.Load<Texture2D> (StringTexture);
		}

		public void Initialize ()
		{
			throw new System.NotImplementedException ();
		}

		public Color? UseColor { get { return Color.White; } }

		public float Depth { get { return Depths.Background; } }

		public void Draw (RectangleF area, SpriteBatch bat)
		{
			// TODO: Implementar la extensión Draw con RectangleF
			var ar = area.ToRectangle ();
			bat.Draw (
				Texture,
				ar, null, Color.White,
				0, Vector2.Zero,
				SpriteEffects.None,
				Depths.Background);
		}

		public Point Location { get; set; }

		public void Dispose ()
		{
			Texture = null;
		}
	}
}
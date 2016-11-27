using System;
using Items;
using Skills;
using Units;

namespace Skills
{
	/// <summary>
	/// Representa el efecto de un <see cref="Units.Skills.ISkill"/> que consisnte en eliminar un item de un <see cref="IUnidad"/>
	/// </summary>
	public class RemoveItemEffect : ITargetEffect
	{
		/// <summary>
		/// Runs the effect
		/// </summary>
		public void Execute (bool checkRun)
		{
			if (this.CheckAndExecute ())
			{
				// Removes the item, it it does not exists: exception
				if (!Target.Inventory.Items.Remove (RemovingItem))
					throw new Exception ("Cannot execute effect");
			}
		}

		EffectResultEnum result;

		/// <summary>
		/// Devuelve el resultado del hit-check
		/// </summary>
		/// <value>The result.</value>
		public EffectResultEnum Result
		{
			get
			{
				return result;
			}
			set
			{
				if (result != EffectResultEnum.NotInstanced)
					throw new InvalidOperationException ();
				result = value;
			}
		}

		/// <summary>
		/// Devuelve un <c>string</c> de una línea que describe este efecto como infobox
		/// </summary>
		public string DetailedInfo ()
		{
			return string.Format ("{1}: Removes {0}", RemovingItem.Nombre, Chance);
		}

		/// <summary>
		/// Probabilidad de que ocurra
		/// </summary>
		public double Chance { get; set; }

		/// <summary>
		/// Whose inventory gonna lose the item.
		/// </summary>
		public IUnidad Target { get; }

		ITarget ITargetEffect.Target { get { return Target; } }

		/// <summary>
		/// Item to remove
		/// </summary>
		public IItem RemovingItem { get; }

		/// <summary>
		/// What caused this effect
		/// </summary>
		public IEffectAgent Agent { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Skills.RemoveItemEffect"/> class.
		/// </summary>
		/// <param name="source">Source.</param>
		/// <param name="target">Target.</param>
		/// <param name="removingItem">Removing item.</param>
		public RemoveItemEffect (IEffectAgent source,
		                         IUnidad target,
		                         IItem removingItem)
		{
			Agent = source;
			Target = target;
			RemovingItem = removingItem;
		}
	}
}
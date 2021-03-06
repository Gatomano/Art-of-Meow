﻿using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Units.Buffs;

namespace Items.Declarations.Equipment
{
	/// <summary>
	/// Armadura equipable genérica.
	/// Puede cambiar los stats asignando un diccionario
	/// </summary>
	public class GenericArmor : Equipment, IBuffGenerating
	{
		/// <summary>
		/// Diccionario que contiene los cambios en los stats (por nombre único) en quien lo lleva puesto
		/// </summary>
		/// <value>The delta dict.</value>
		protected Dictionary<string, float> DeltaDict { get; }

		/// <summary>
		/// Slot que ocupa esta armadura
		/// </summary>
		public override EquipSlot Slot { get; }

		IEnumerable<KeyValuePair<string, float>> IBuffGenerating.GetDeltaStat ()
		{
			return DeltaDict;
		}

		/// <summary>
		/// Gets the value or worth of the item
		/// </summary>
		public override float Value
		{
			get
			{
				// TODO: Calcular bien
				return base.Value + DeltaDict.Values.Sum ();
			}
		}

		/// <summary>
		/// Clone this instance.
		/// </summary>
		public override object Clone ()
		{
			return new GenericArmor (NombreBase, Slot, DeltaDict)
			{
				TextureName = TextureName,
				Texture = Texture,
				Color = Color,
				AllowedModNames = AllowedModNames,
			};
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Items.Declarations.Equipment.GenericArmor"/> class.
		/// </summary>
		/// <param name="NombreBase">Nombre</param>
		/// <param name="Slot">Slot</param>
		/// <param name = "DeltaDict">The attributes that this item modifies</param>
		[JsonConstructor]
		public GenericArmor (string NombreBase, EquipSlot Slot, Dictionary<string, float> DeltaDict = null)
			: base (NombreBase)
		{
			this.DeltaDict = DeltaDict ?? new Dictionary<string, float> ();
			this.Slot = Slot;
		}
	}
}
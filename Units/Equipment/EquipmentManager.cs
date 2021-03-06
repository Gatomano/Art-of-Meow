﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Debugging;
using Items;
using Items.Declarations.Equipment;
using Microsoft.Xna.Framework.Content;
using Units.Buffs;

namespace Units.Equipment
{
	/// <summary>
	/// Equipment manager: manages buff and equip/unequip of the equipment of <see cref="IUnidad"/>
	/// </summary>
	public class EquipmentManager
	{
		/// <summary>
		/// El poseedor del equipment
		/// </summary>
		/// <value>The owner.</value>
		public IUnidad Owner { get; }

		/// <summary>
		/// Devuelve el buff que representa el equipment.
		/// </summary>
		public EquipBuff EquipBuff { get; private set; }

		/// <summary>
		/// Enumerates all the equiped <see cref="IEquipment"/> in the corresponging <see cref="IUnidad"/>
		/// </summary>
		/// <returns>The equipment.</returns>
		public IEnumerable<IEquipment> EnumerateEquipment ()
		{
			return equipment;
		}

		/// <summary>
		/// If any, gets the first melee weapon in equipment; otherwise gets a <see cref="FistMeleeEffect"/> for this Unidad
		/// </summary>
		public IMeleeEffect GetMeleeDamageType ()
		{
			var melees = equipment.OfType<IMeleeEffect> ();
			return melees.Any () ? melees.First () : new FistMeleeEffect ();
		}

		List<IEquipment> equipment { get; }

		/// <summary>
		/// Equipa un item
		/// </summary>
		public void EquipItem (IEquipment equip)
		{
			if (equip.Owner != null)
				throw new InvalidOperationException ("equip no debe estar equipado para equiparse.");
			if (CurrentSlotCount (equip.Slot) < SlotSize [equip.Slot])
			{
				equipment.Add (equip);
				equip.Owner = this;
				AgregadoEquipment?.Invoke (this, equip);
			}
			else
			{
				Debug.WriteLine (
					"Hay un conflicto de equipment con " + equip.NombreBase,
					DebugCategories.Equipment);
			}
		}

		/// <summary>
		/// Desequipa un item.
		/// </summary>
		/// <param name="equip">Equip.</param>
		public void UnequipItem (IEquipment equip)
		{
			if (equip.Owner != this)
				throw new InvalidOperationException ("Objeto no está equipado.");

			EliminadoEquipment?.Invoke (this, equip);
			equip.Owner = null;
			equipment.Remove (equip);

			// Mandar item desequipado a Inventory
			Owner.Inventory.Items.Add (equip);
		}

		/// <summary>
		/// Devuelve el número de items equipados en un slot dado.
		/// </summary>
		public int CurrentSlotCount (EquipSlot slot)
		{
			return equipment.Count (z => z.Slot == slot);
		}

		/// <summary>
		/// Initializes the content of its elements
		/// </summary>
		protected void LoadContent (ContentManager manager)
		{
			foreach (var eq in equipment)
				eq.LoadContent (manager);
		}

		#region Events

		/// <summary>
		/// Ocurre al agregar un nuevo equipment.
		/// </summary>
		public event EventHandler<IEquipment> AgregadoEquipment;

		/// <summary>
		/// Ocurre al eliminar un equipment.
		/// </summary>
		public event EventHandler<IEquipment> EliminadoEquipment;

		#endregion

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Units.Equipment.EquipmentManager"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Units.Equipment.EquipmentManager"/>.</returns>
		public override string ToString ()
		{
			return string.Format (
				"[EquipmentManager: Owner={0}, EquipBuff={1}, equipment={2}]",
				Owner,
				EquipBuff,
				equipment);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Units.Equipment.EquipmentManager"/> class.
		/// </summary>
		/// <param name="owner">Owner.</param>
		public EquipmentManager (IUnidad owner)
		{
			Owner = owner;
			equipment = new List<IEquipment> ();
			EquipBuff = new EquipBuff (equipment);
		}

		#region Static

		/// <summary>
		/// Gets a dictionary that assigns every slot the quantity of that kind that can be equiped
		/// </summary>
		public static Dictionary<EquipSlot, int> SlotSize;

		static EquipmentManager ()
		{
			SlotSize = new Dictionary<EquipSlot, int> ();
			SlotSize.Add (EquipSlot.None, 0);
			SlotSize.Add (EquipSlot.Head, 1);
			SlotSize.Add (EquipSlot.Body, 1);
			SlotSize.Add (EquipSlot.MainHand, 1);
		}

		#endregion
	}
}
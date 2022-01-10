﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using PrimitierModdingFramework.Internals;

namespace PrimitierModdingFramework
{
	public class PMFHelper : PMFSystem
	{
		public static Transform LHand;
		public static Transform RHand;
		public static Transform CameraRig;
		public static PlayerMovement PlayerMovement;

		/// <summary>
		/// The system transform is used to store gameobjects that don't require saving (Custom objects that are not Cubes)
		/// </summary>
		public static Transform SystemTransform;


		public override void OnSceneLoad()
		{
			SystemTransform = GameObject.Find("System").transform;

			CameraRig = GameObject.Find("[CameraRig]").transform;
			LHand = GameObject.Find("LeftHand").transform;
			RHand = GameObject.Find("RightHand").transform;

			PlayerMovement = CameraRig.GetComponent<PlayerMovement>();
			

		}

	}
}

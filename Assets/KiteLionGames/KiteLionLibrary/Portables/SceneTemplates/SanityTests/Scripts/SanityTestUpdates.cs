#region FileHeader

// test
// Project: Assembly-CSharp
// File:    SanityTestUpdates.cs
// Author:  Eliot CS
// Created: 2024.09.18.01.09.15
// Edited: 2024.09.19.01.09.57
//
// Copyright (c) 2024 SomeGameDevs, LLC. All rights reserved.
//
// This source code is the property of SomeGameDevs, LLC and may not be
// copied, distributed, modified, or used in any way without prior written
// permission from SomeGameDevs, LLC.
//
// Description:
// [Provide a brief description of what this file/class does.]
//
// Previous Header (if any):
//
// License:
// This code is provided "as is," without warranty of any kind, express or
// implied, including but not limited to the warranties of merchantability,
// fitness for a particular purpose, and noninfringement. In no event shall
// the authors or copyright holders be liable for any claim, damages, or
// other liability, whether in an action of contract, tort, or otherwise,
// arising from, out of, or in connection with the software or the use or
// other dealings in the software.

#endregion

/// <summary>
/// SanityTestScript
/// License: GNU AGPLv3
/// Copyright (C) 2022 KiteLion Games
///
/// This program is free software: you can redistribute it and/or modify
/// it under the terms of the GNU Affero General Public License as published
/// by the Free Software Foundation, either version 3 of the License, or
/// (at your option) any later version.
///
/// This program is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU Affero General Public License for more details.
///
/// You should have received a copy of the GNU Affero General Public License
/// along with this program.  If not, see <https://www.gnu.org/licenses/>.
///
/// Contact: support@kiteliongames.com
/// </summary>

using UnityEngine;

namespace KiteLionGames.KiteLionLibrary.Portables.SceneTemplates.SanityTests.Scripts
{
    public class SanityTestScript : MonoBehaviour
    {
        public int SanityCheckCount;
        public float UpdatesPerSecond;
        private readonly float nowTime = 0.0f;

        private float startTime;

        // Start is called before the first frame update
        private void Start()
        {
            Debug.Log("Your scene started!");
        }

        // Update is called once per frame
        private void Update()
        {
            SanityCheckCount++;

            if (startTime == 0.0f)
            {
                startTime = Time.time;
            }

            UpdatesPerSecond = SanityCheckCount / (nowTime - startTime);

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }
}

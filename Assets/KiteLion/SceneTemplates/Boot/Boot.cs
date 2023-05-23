/// <summary>
/// Boot
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KiteLionGames;
using UnityEngine.SceneManagement;

public class Boot : MonoBehaviour
{

    public Main.Scenes FirstScene;

    void Start()
    {
        CBUG.LogToFile("Booting ...");
        CBUG.LogToFile("Success!");

        SceneManager.LoadScene(FirstScene.ToString(), LoadSceneMode.Additive);
    }

}

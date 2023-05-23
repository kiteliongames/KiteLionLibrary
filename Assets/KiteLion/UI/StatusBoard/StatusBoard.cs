/// <summary>
/// StatusBoard
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
using KiteLionGames.Common;
using TMPro;
using UnityEngine.UI;

public class StatusBoard : MonoBehaviour
{
    public TMP_Text TextObject;
    public GameObject SB; //Enabled and Disabled Object

    public float TimeAliveDefault;

    private static GameObject selfObject = null;
    private static StatusBoard statusBoard = null;

    // Start is called before the first frame update
    void Start()
    {

        if(selfObject != null || statusBoard != null)
        {
            CBUG.Error("StatusBoard made twice!?!?!? Killing Duplicate ...");
            Destroy(gameObject);
            return;
        }

        selfObject = gameObject;
        statusBoard = gameObject.GetComponent<StatusBoard>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void WriteMessage(string message)
    {
        if(selfObject == null)
        {
            CBUG.Do($"StatusBoard not initiated yet. message attempt was: {message}");
            return;
        }

        statusBoard.ShowStatusBoard();

        statusBoard.TextObject.text = message;

        Tools.DelayFunction(statusBoard.HideStatusBoard, statusBoard.TimeAliveDefault);
    }

    private void HideStatusBoard()
    {
        SB.SetActive(false);
    }

    private void ShowStatusBoard()
    {
        SB.SetActive(true);
    }

}

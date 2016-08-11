using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Practice : MonoBehaviour {

	public string[] name;
	public List<string> players;
	public List<int> costume;
	public List<GameObject> playerObjects;
	public GameObject[] playerPlace;
	public int playerID;
	public int mapChoosen;

    public void Start()
    {
        /*playerID = 0;

        for (int i = 0; i < Network.instance.sfs.LastJoinedRoom.UserList.Count; i++)
        {
            InitPlayer(Network.instance.sfs.LastJoinedRoom.UserList[i].Name,i);
        }*/

        playerID = Random.Range(0, 4);
        //RandomPlayer(0);

        for (int i = 0; i < 4; i++)
        {
            RandomPlayer(i);
        }
    }

    void Update()
    {
        mapChoosen = GameObject.FindObjectOfType<ChooseMap>().mapChoosen;
    }

    void RandomPlayer(int id)
    {
        players.Add(name[Random.Range(0, name.Length)]);
        costume.Add(Random.Range(0, 21));
        GameObject character = Instantiate(ObjectLibrary.instance.characterBase) as GameObject;
        character.transform.parent = transform;
        character.transform.position = playerPlace[id].transform.position - new Vector3(0, 0.3f, 0);
        character.GetComponent<CharacterAnimationController>().preview = true;
        character.GetComponent<CharacterAnimationController>().costume = costume[id];
        character.GetComponent<CharacterAnimationController>().SetSortingLayer(10000);
        character.GetComponent<CharacterAnimationController>().SetFaceDirection(new KeyValuePair<float, bool>(270, true));
        character.name = players[id];

        playerObjects.Add(character);
    }

    void InitPlayer(string name, int index)
    {
        players.Add(name);
        costume.Add(0);
        GameObject character = Instantiate(ObjectLibrary.instance.characterBase) as GameObject;
        character.transform.parent = transform;
        character.transform.position = playerPlace[index].transform.position - new Vector3(0, 0.3f, 0);
        character.GetComponent<CharacterAnimationController>().preview = true;
        character.GetComponent<CharacterAnimationController>().costume = costume[index];
        character.GetComponent<CharacterAnimationController>().SetSortingLayer(10000);
        character.GetComponent<CharacterAnimationController>().SetFaceDirection(new KeyValuePair<float, bool>(270, true));
        character.name = name;

        playerObjects.Add(character);
    }

	public void Shuffle(){
		players.Clear ();
		costume.Clear ();
		foreach(GameObject go in playerObjects)Destroy(go);
		playerObjects.Clear ();
		Start ();
	}

	public void Play(){
		GameData.instance.gameState = GameData.GameState.PRACTICE_GAMEPLAY;
		Destroy (GameData.instance.currentScene);
		GameData.instance.currentScene = Instantiate (ObjectLibrary.instance.maps[mapChoosen])as GameObject;
		GameData.instance.currentScene.name = ObjectLibrary.instance.maps[mapChoosen].name;
		GameData.instance.currentScene.GetComponent<MatchManager>().PreparePracticeGame (players, costume, playerID);
	}
}

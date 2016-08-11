using UnityEngine;
using System.Collections;
using System;
public class Achievement : MonoBehaviour {

	public static Achievement instance;

	//Daily Achievement
	public int logInDaily;
	public int pvpGamesDaily;
	public int killDroneDaily;
	public int killEnemyDaily;
	
	//Weekly Achievement
	public int logInWeekly;
	public int pvpGamesWeekly;
	public int killDroneWeekly;
	public int killEnemyWeekly;
	public int blackBuffWeekly;
	
	//Monthly Achievement
	public int logInMonthly;
	public int pvpGamesMonthly;
	public int killDroneMonthly;
	public int killEnemyMonthly;
	public int destroyBunkerMonthly;

	//Gameplay Achievement
	public int killTime;

	//Date system
	public DateTime lastLogin;
	public DateTime lastLoginBefore;
	public int day;
	public int week;
	public int month;


	public void Login(){
		if (day != DateTime.Now.Day) {
			logInDaily += 1;
			logInWeekly += 1;
			logInMonthly += 1;
			day = DateTime.Now.Day;
		}

	}

	public void PlayPVP(){
		pvpGamesDaily += 1;
		pvpGamesWeekly += 1;
		pvpGamesMonthly += 1;
	}

	public void KillEnemy(){
		killEnemyDaily += 1;
		killEnemyWeekly += 1;
		killEnemyMonthly += 1;
	}

	public void KillDrone(){
		killDroneDaily += 1;
		killDroneWeekly += 1;
		killDroneMonthly += 1;
	}

	public void KillBlackBuff(){
		blackBuffWeekly += 1;
	}

	public void DestroyBunker(){
		destroyBunkerMonthly += 1;
	}

	public void InitializeAchievement(){
		SetAchievementData ();
	}

	public void ResetAchievementData(){
		day = PlayerPrefs.GetInt ("date");
		week = PlayerPrefs.GetInt ("week");
		month = PlayerPrefs.GetInt ("month");
		
		if(day != DateTime.Now.Day){
			ResetAchievementData("daily");
			day = DateTime.Now.Day;
			PlayerPrefs.SetInt("date", day);
		}
		if(week != Mathf.FloorToInt(DateTime.Now.Day / 7)){
			ResetAchievementData("week");
			week = Mathf.FloorToInt(DateTime.Now.Day/7);
			PlayerPrefs.SetInt("week", week);
		}
		if(month != DateTime.Now.Month){
			ResetAchievementData("month");
			month = DateTime.Now.Month;
			PlayerPrefs.SetInt("month", month);
		}
	}

	public void SetAchievementData(){
		//Daily Achievement
		string dailyAchievementLong = PlayerPrefs.GetString ("dailyAchievement");
		string[] dailyAchievement = dailyAchievementLong.Split (new char['|']);
		if(dailyAchievementLong != ""){
			int.TryParse(dailyAchievement[0], out logInDaily);
			int.TryParse(dailyAchievement[1], out pvpGamesDaily);
			int.TryParse(dailyAchievement[2], out killDroneDaily);
			int.TryParse(dailyAchievement[3], out killEnemyDaily);
		}

		//Weekly Achievement
		string weeklyAchievementLong = PlayerPrefs.GetString ("weeklyAchievement");
		string[] weeklyAchievement = weeklyAchievementLong.Split (new char['|']);
		if(weeklyAchievementLong != ""){
			int.TryParse(dailyAchievement[0], out logInWeekly);
			int.TryParse(dailyAchievement[1], out pvpGamesWeekly);
			int.TryParse(dailyAchievement[2], out killDroneWeekly);
			int.TryParse(dailyAchievement[3], out killEnemyWeekly);
			int.TryParse(dailyAchievement[3], out blackBuffWeekly);
		}

		//Monthly
		string monthlyAchievementLong = PlayerPrefs.GetString ("monthlyAchievement");
		string[] monthlyAchievement = monthlyAchievementLong.Split (new char['|']);
		if(monthlyAchievementLong != ""){
			int.TryParse(dailyAchievement[0], out logInMonthly);
			int.TryParse(dailyAchievement[1], out pvpGamesMonthly);
			int.TryParse(dailyAchievement[2], out killDroneMonthly);
			int.TryParse(dailyAchievement[3], out killEnemyMonthly);
			int.TryParse(dailyAchievement[4], out destroyBunkerMonthly);
		}
	}

	public void ResetAchievementData(string dataToReset){
		if (dataToReset == "daily") {
			logInDaily = 0;
			pvpGamesDaily = 0;
			killDroneDaily = 0;
			killEnemyDaily = 0;
		}else if (dataToReset == "weekly") {
			logInWeekly = 0;
			pvpGamesWeekly = 0;
			killDroneWeekly = 0;
			killEnemyWeekly = 0;
			blackBuffWeekly = 0;
		}else if (dataToReset == "monthly") {
			logInMonthly = 0;
			pvpGamesMonthly = 0;
			killDroneMonthly = 0;
			killEnemyMonthly = 0;
			destroyBunkerMonthly = 0;
		}
	}

	public void SaveAchievementData(){
		string dailyData = logInDaily.ToString () + "|" + pvpGamesDaily.ToString () + "|" +
						   killDroneDaily.ToString () + "|" + killEnemyDaily.ToString ();

		string weeklyData = logInWeekly.ToString() + "|" + pvpGamesWeekly.ToString() + "|" +
						    killDroneWeekly.ToString() + "|" + killEnemyWeekly.ToString() + "|" + blackBuffWeekly.ToString();

		string monthlyData = logInMonthly.ToString() + "|" + pvpGamesMonthly.ToString() + "|" + killDroneMonthly.ToString() + "|" +
							 killEnemyMonthly.ToString() + "|" + destroyBunkerMonthly.ToString();

		PlayerPrefs.SetString ("dailyAchievement", dailyData);
		PlayerPrefs.SetString ("weeklyAchievement", dailyData);
		PlayerPrefs.SetString ("monthlyAchievement", dailyData);
	}

	public void Awake(){
		instance = this;
	}

	public void Update(){
		if(instance == null) instance = this;
	}


}

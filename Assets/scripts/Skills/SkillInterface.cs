using UnityEngine;
using System.Collections;
using System.Collections.Generic;

interface SkillInterface{
	void InsertSelection(Transform map);
	IList GetSelectionRange();
	void Execute();
}

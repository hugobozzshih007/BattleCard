using UnityEngine;
using System.Collections;
using System.Collections.Generic;

interface CommonSkill{
	void InsertSelection(Transform map);
	IList GetSelectionRange();
	void Execute();
}

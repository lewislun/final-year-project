public static class StringOperation {
	public static string ToFirstUpper(string str){
		if (str == null)
			return "";
		string result = "";
		if (str.Length > 0)
			result += str.Substring(0,1).ToUpper();
		if (str.Length > 1)
			result += str.Substring(1,str.Length - 1).ToLower();
		
		return result;
	}
}
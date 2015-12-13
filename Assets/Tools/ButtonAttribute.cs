// http://baba-s.hatenablog.com/entry/2014/07/30/152826

using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Custom {

	/// <summary>
	/// Inspector に GUI.Button を表示して、指定された関数を実行したい場合はこの ButtonAttribute を使用してください
	/// </summary>
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
	public sealed class ButtonAttribute : PropertyAttribute
	{
		public string   Function    { get; private set; }   // 関数名
		public string   Name        { get; private set; }   // ボタンに表示するテキスト
		public object[] Parameters  { get; private set; }   // 関数に渡す引数を管理する配列

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="function">関数名</param>
		/// <param name="name">ボタンに表示するテキスト</param>
		/// <param name="parameters">関数に渡す引数を管理する配列</param>
		public ButtonAttribute( string function, string name, params object[] parameters )
		{
			Function    = function;
			Name        = name;
			Parameters  = parameters;
		}
	}

	#if UNITY_EDITOR

	[CustomPropertyDrawer(typeof(ButtonAttribute))]
	public sealed class ButtonDrawer : PropertyDrawer
	{
		public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
		{
			var buttonAttribute = attribute as ButtonAttribute;

			if ( GUI.Button( position, buttonAttribute.Name ) )
			{
				var objectReferenceValue    = property.serializedObject.targetObject;
				var type                    = objectReferenceValue.GetType();
				var bindingAttr             = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
				var method                  = type.GetMethod( buttonAttribute.Function, bindingAttr );

				try
				{
					method.Invoke( objectReferenceValue, buttonAttribute.Parameters );
				}
				catch ( AmbiguousMatchException )
				{
					var format  = @"{0}.{1} 関数がオーバーロードされているため関数を特定できません。{0}.{1} 関数のオーバーロードを削除してください";
					var message = string.Format( format, type.Name, buttonAttribute.Function );

					Debug.LogError( message, objectReferenceValue );
				}
				catch ( ArgumentException )
				{
					var parameters  = string.Join( ", ", buttonAttribute.Parameters.Select( c => c.ToString() ).ToArray() );
					var format      = @"{0}.{1} 関数に引数 {2} を渡すことができません。{0}.{1} 関数の引数の型が正しいかどうかを確認してください";
					var message     = string.Format( format, type.Name, buttonAttribute.Function, parameters );

					Debug.LogError( message, objectReferenceValue );
				}
				catch ( NullReferenceException )
				{
					var format  = @"{0}.{1} 関数は定義されていません。{0}.{1} 関数が定義されているかどうかを確認してください";
					var message = string.Format( format, type.Name, buttonAttribute.Function );

					Debug.LogError( message, objectReferenceValue );
				}
				catch ( TargetParameterCountException )
				{
					var parameters  = string.Join( ", ", buttonAttribute.Parameters.Select( c => c.ToString() ).ToArray() );
					var format      = @"{0}.{1} 関数に引数 {2} を渡すことができません。{0}.{1} 関数の引数の数が正しいかどうかを確認してください";
					var message     = string.Format( format, type.Name, buttonAttribute.Function, parameters );

					Debug.LogError( message, objectReferenceValue );
				}
			}
		}
	}

	#endif
}

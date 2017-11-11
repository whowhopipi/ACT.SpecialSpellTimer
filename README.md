**ACT.SpecialSpellTimer**
=====================
# 移行しました
ACT.SpecialSpellTimer, ACT.TTSYukkuri, ACT.UltraScouter は配布を統合し下記のプロジェクトに移行しました。  
**ACT.Hojoring**  
**https://github.com/anoyetta/ACT.Hojoring**  
こちらから最新版を取得してください。

# 概要
見やすさを改善した特別なスペルタイマーを提供します。  
愛称は「スペスペ」  

![sample](https://raw.githubusercontent.com/anoyetta/ACT.SpecialSpellTimer/master/sample.png)  

# 使い方
## 1. 準備
**[.NET Framework 4.7](https://www.microsoft.com/en-us/download/details.aspx?id=55170)** をインストールします。  
※スペスペの動作には. NET Framework 4.7 以降が必要です。  

## 2. インストール
<pre><b>resources
tools
FFXIV.Framework.Dialog.exe
ACT.SpecialSpellTimer.Core.dll
ACT.SpecialSpellTimer.dll
FFXIV.Framework.Dialog.Wrapper.dll
FFXIV.Framework.dll
FirstFloor.ModernUI.dll
NLog.dll
Prism.dll
</b></pre>
をACTのインストールディレクトリにコピーします  
　  
(配置の例)  
![deploy_sample](https://raw.githubusercontent.com/anoyetta/ACT.SpecialSpellTimer/master/images/deploy.jpg)  
その後、プラグインとしてACT.SpecialSpellTimer.dllを追加してください  

## DoTなどの設定方法が分からない
**[こちら](https://github.com/anoyetta/ACT.Hojoring/wiki/SpecialSpellTimer_Examples)** に高度な設定例を示します。  
正規表現をフル活用することになりますので各正規表現の意味を理解して使用してください。

## そもそも正規表現が分からない
基本的には分からなくてよいです。正規表現とは専らプログラマなどがテキストを効率よく検索するときに使用する高度な検索キーワードで、開発用のエディタ内やプログラムの内部に埋め込んで使います。  
よって一般の方には使いこなすのは難しいです。分からない場合は通常のキーワードマッチングの範囲内で使用するのがベターです。  

#### どうしても正規表現を使いたい
下記で学習できます。  

* 正規表現とは？  
[正規表現 - Wikipedia](https://ja.wikipedia.org/wiki/正規表現)  
ざっくり説明すると \* などのワイルドカードをより強力にしたものです。C#等のプログラミング言語とは独立して体系化されているため、C#, JavaScript, PHP, Bash など言語を問わず使用できます。

* 正規表現のリファレンス  
[正規表現の構文 - MSDN](https://msdn.microsoft.com/ja-jp/library/cc392020.aspx)  
[正規表現言語 クイックリファレンス - MSDN](https://msdn.microsoft.com/ja-jp/library/az24scfc(v=vs.110).aspx)  
マイクロソフトの開発者向け情報（MSDN - Microsoft Developer Network）の正規表現のページです。分かりやすくまとまっているため学習には最適です。

* 正規表現のテスト  
正規表現を書いていきなりゲームのログとマッチさせようとしてもうまくいかないでしょう。またマッチしない原因を探ることも出来ないでしょう。正規表現を書いたらチェックツール等を使用して正規表現をテストしましょう。  
[正規表現テストツール - DOBON.NET](https://dobon.net/vb/dotnet/string/files/RegexTest.exe)  
まずはスペスペでログを収集します。ヒットさせたいログが出力されていることを確認し、そのテキストを対象にして自分の書いた正規表現を前述のテストツールでテストします。これが基本です。  
仮にプログラマだったとしても複雑な文字列に対してはテストしながらパターンを作ります。よって誰かに質問する場合も検査したいテキストを添えて「このテキストに対してこのパターンで正規表現を設定したがマッチしない」と質問するとよいでしょう。

## ゲーム内のプレースホルダは使えないの？
一部は使えるように対応しています

<table>
<tr>
<td>&lt;me&gt;</td>
<td>使えます。<br />ただし、イニシャルにはマッチしません。</td>
</tr>

<tr>
<td>&lt;2&gt;～&lt;8&gt;</td>
<td>
使えます。<br />ただし、イニシャルにはマッチしません。<br />
オプションにてパーティメンバ代名詞をONにしてください。
</td>
</tr>

<tr>
<td>&lt;mex&gt;</td>
<td>
&lt;me&gt;の拡張版です。フルネーム、イニシャルを問わずプレイヤーにマッチします。<br />
正規表現をONにしてください。<br />
<br />
ex. プレイヤーが Naoki Yoshida の例<br />
(?<_mex>Naoki Yoshida|Naoki Y\.|N\. Yoshida|N\. Y\.)<br />
に置換わってマッチングされます。
</td>
</tr>

<tr>
<td>&lt;2ex&gt;～&lt;8ex&gt;</td>
<td>
&lt;2&gt;～&lt;8&gt;の拡張版です。フルネーム、イニシャルを問わずプレイヤーにマッチします。<br />
正規表現とパーティメンバ代名詞の両方をONにしてください。<br />
<br />
ex. パーティリストの2番目が Naoki Yoshida の例<br />
(?<_2ex>Naoki Yoshida|Naoki Y\.|N\. Yoshida|N\. Y\.)<br />
に置換わってマッチングされます。
</td>
</tr>

<tr>
<td>&lt;t&gt;</td>
<td>負荷が高いため搭載できません</td>
</tr>

<tr>
<td>&lt;tt&gt;</td>
<td>負荷が高いため搭載できません</td>
</tr>

<tr>
<td>&lt;ft&gt;</td>
<td>負荷が高いため搭載できません</td>
</tr>

<tr>
<td>&lt;petid&gt;</td>
<td>
FF14が内部のオブジェクトに割当てている一意なIDに置換されます<br />
このIDによってACTが生成している詳細なログにマッチさせると自分のペットだけを識別出来ます
</td>
</tr>

<tr>
<td>&lt;JOB&gt;, &lt;JOBn&gt;</td>
<td>
パーティ内の特定のジョブの誰か、または特定のジョブのｎ番目のメンバーにマッチします。<br />
フルネーム、イニシャルを問わずマッチします。<br />
正規表現とパーティメンバ代名詞の両方をONにしてください。<br />
<br />
ex. パーティメンバーが下記のとき・・・<br />
Taro Paradin (ナイト)<br />
Jiro Paradin (ナイト)<br />
<br />
&lt;PLD1&gt; → Taro Paradin にマッチする<br />
&lt;PLD2&gt; → Jiro Paradin にマッチする<br />
&lt;PLD&gt; → Taro Paradin または Jiro Paradin にマッチする<br />
<br />
&lt;PLD1&gt; は正規表現の (?&lt;_PLD1&gt;Taro Paladin|Taro P\.|～省略) に置換わってマッチングされます。<br />
&lt;PLD&gt; は正規表現の (?&lt;_PLD&gt;Taro Paladin|Jiro Paladin|～省略) に置換わってマッチングされます。<br />
<br />
</td>
</tr>

<tr>
<td>&lt;ROLE&gt;, &lt;ROLEn&gt;</td>
<td>
パーティ内の特定のロールの誰かにマッチします。<br />
フルネーム、イニシャルを問わずマッチします。<br />
正規表現とパーティメンバ代名詞の両方をONにしてください。<br />
<br />
ex. パーティメンバーが下記のとき・・・<br />
Taro Yamada (ナイト)<br />
Jiro Sato (戦士)<br />
Sabro Suzuki (白魔道士)<br />
Shiro Honda (学者)<br />
Goro Toyota (モンク)<br />
Rokuro Nissan (竜騎士)<br />
Shichiro Mazda (吟遊詩人)<br />
Hachiro Mitsuoka (黒魔道士)<br />
<br />
&lt;TANK&gt; → Taro Yamada または Jiro Sato にマッチする<br />
&lt;TANK1&gt; → 1番目のTANKである Taro Yamada にマッチする<br />
&lt;HEALER&gt; → Sabro Suzuki または Sabro Suzuki にマッチする<br />
&lt;DPS&gt; → Goro Toyota または Rokuro Nissan または Shichiro Mazda または Hachiro Mitsuoka にマッチする<br />
&lt;MELEE&gt; → Goro Toyota または Rokuro Nissan にマッチする<br />
&lt;RANGE&gt; → Shichiro Mazda にマッチする<br />
&lt;MAGIC&gt; → Hachiro Mitsuoka にマッチする<br />
<br />
&lt;TANK&gt; は正規表現の (?&lt;_TANK&gt;Taro Yamada|Jiro Sato) に置換わってマッチングされます。<br />
&lt;TANK1&gt; は正規表現の (?&lt;_TANK1&gt;Taro Yamada) に置換わってマッチングされます。<br />
ex.<br />
&lt;TANK&gt → (?&lt;_TANK&gt;Taro Yamada|Jiro Sato)<br />
&lt;TANK1&gt → (?&lt;_TANK&gt;Taro Yamada)<br />
&lt;HEALER&gt → (?&lt;_HEALER&gt;Sabro Suzuki|Sabro Suzuki)<br />
&lt;HEALER1&gt → (?&lt;_HEALER&gt;Sabro Suzuki)<br />
&lt;DPS&gt → (?&lt;_DPS&gt;Goro Toyota|Rokuro Nissan|Shichiro Mazda|Hachiro Mitsuoka)<br />
&lt;DPS1&gt → (?&lt;_DPS&gt;Goro Toyota)<br />
&lt;MELEE&gt → (?&lt;_MELEE&gt;Goro Toyota|Rokuro Nissan)<br />
&lt;RANGE&gt → (?&lt;_RANGE&gt;Shichiro Mazda)<br />
&lt;MAGIC&gt → (?&lt;_MAGIC&gt;Hachiro Mitsuoka)<br />
<br />
</td>
</tr>

</table>
各種プレースホルダの詳細はゲーム実行中に Log タブで確認出来ます。    
<br />
<br />

## 俺の歌を聞かせたい
resources/wav にwaveファイルを投入するとスペスペで使用できるようになります  

# テキストコマンド
FF14の内部からテキストコマンドで一部の機能を制御できます  
/e コマンド  
の書式でコマンドを発行してください  

例) 全てのスペルを無効にする  
/e /spespe changeenabled spells all false  
  
<table>
<tr>
<td>コマンド</td><td>説明</td>
</tr>

<tr>
<td>/spespe on</td>
<td>スペスペのオーバーレイの表示を有効にする（スペスペボタンONと同様）</td>
</tr>

<tr>
<td>/spespe off</td>
<td>スペスペのオーバーレイの表示を無効にする（スペスペボタンOFFと同様）</td>
</tr>

<tr>
<td>/spespe refresh spells</td>
<td>スペルリストパネルを一度閉じてリフレッシュする</td>
</tr>

<tr>
<td>/spespe refresh telops</td>
<td>テロップを一度閉じてリフレッシュする</td>
</tr>

<tr>
<td>/spespe refresh me</td>
<td>プレイヤー名のキャッシュを更新する</td>
</tr>

<tr>
<td>/spespe refresh pt</td>
<td>パーティメンバー名のキャッシュを更新する</td>
</tr>

<tr>
<td>/spespe refresh pet</td>
<td>自身のペットIDのキャッシュを更新する</td>
</tr>

<tr>
<td>/spespe changeenabled spells "サンプルパネル" true</td>
<td>指定したパネルのスペルを有効にする。falseで無効</td>
</tr>

<tr>
<td>/spespe changeenabled spells "サンプルスペル" true</td>
<td>指定したスペルを有効にする。falseで無効</td>
</tr>

<tr>
<td>/spespe changeenabled telops "サンプルテロップ" true</td>
<td>指定したテロップを有効にする。falseで無効</td>
</tr>

<tr>
<td>/spespe changeenabled spells all true</td>
<td>全てのスペルを有効にする。falseで無効</td>
</tr>

<tr>
<td>/spespe changeenabled telops all true</td>
<td>全てのテロップを有効にする。falseで無効</td>
</tr>

<tr>
<td>/spespe analyze on</td>
<td>戦闘ログの収集を有効にする。offで無効</td>
</tr>

<tr>
<td>/spespe set placeholder "tag" 文字列</td>
<td>&lt;tag&gt;から文字列への置き換えを有効にする</td>
</tr>

<tr>
<td>/spespe clear placeholder "tag"</td>
<td>&lt;tag&gt;による置換を無効にする</td>
</tr>

<tr>
<td>/spespe clear placeholder all</td>
<td>全ての置換を無効にする</td>
</tr>

</table>

# 最新リリース
**[こちらからダウンロードしてください](https://github.com/anoyetta/ACT.SpecialSpellTimer/releases/latest)**  

# ライセンス
三条項BSDライセンス  
Copryright (c) 2014, anoyetta  
https://github.com/anoyetta/ACT.SpecialSpellTimer/blob/master/LICENSE  

# Special Thanks
・GB19xx様  
https://github.com/GB19xx/ACT.TPMonitor  
のFF14ヘルパークラスを流用させていただきました  

・魔王魂様  
http://maoudamashii.jokersounds.com/  
音楽素材といったら魔王魂。  
同梱されたwaveサウンドファイルの著作権は魔王魂に帰属します  

# お問合せ
不具合報告、要望、質問及び最新版情報などはTwitterにて  
GitHubと連動しているためツイートは少々五月蠅いかもしれません  
https://twitter.com/anoyetta  

**Discord はじめました**  
**ACT.SST Forum**  
**https://discord.gg/n6Mut3F**  
気軽にご参加ください。twitterよりもコミュニケーションし易いです。  
公開では対応できないような案件の場合は別途プライベートルームに招待いたします。  

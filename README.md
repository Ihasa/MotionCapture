MotionCapture
=============

学部のときに作ったやつを上げてみる

XNA Frameworkのランタイムが入っていてKinect for WindowsがつながっているPCであれば動く。
Kinectからデータを拾って簡単なbvhファイルに出力する。

が、bvhファイルにはrootボーン以外の動きを位置データとして出力しているので、CGソフトに読み込んでも正しく動作しない。

[![release](https://img.shields.io/badge/Unity-2020.0+-white.svg?style=flat&logo=unity)](https://github.com/negi0109/ColorGradientToTexture)
[![release](https://github.com/negi0109/ColorGradientToTexture/actions/workflows/release.yml/badge.svg)](https://github.com/negi0109/ColorGradientToTexture/actions/workflows/release.yml)
# ColorGradientToTexture

Unity上で動く画像生成ツール

## Install

Unity Package Manager から追加

https://github.com/negi0109/ColorGradientToTexture/releases/latest

## Examples

<table>
    <tr>
        <td>
            <img width="100" src="https://user-images.githubusercontent.com/33025461/128602677-35cf659e-bb9a-4b68-8e33-3f67faf558bf.png">
        </td>
        <td>
            <img width="100" src="https://user-images.githubusercontent.com/33025461/128602880-bca78454-f942-4afc-93e6-b1f0f03769af.png">
        </td>
        <td>
            <img width="100" src="https://user-images.githubusercontent.com/33025461/128602988-9842f8d5-f7d1-4ba8-94d7-ee3c0f84bf43.png">
        </td>
        <td>
            <img width="100" src="https://user-images.githubusercontent.com/33025461/128603059-3af55e5e-ea28-4e43-b13f-e43a20e57a70.png">
        </td>
        <td>
            <img width="100" src="https://user-images.githubusercontent.com/33025461/128603142-e63b0c6f-acf1-4ee2-8b50-d93d130832ba.png">
        </td>
        <td>
            <img width="100" src="https://user-images.githubusercontent.com/33025461/128637133-181b4d86-f816-45e7-980b-309879655f1b.png">
        </td>
    </tr>
    <tr>
        <td>
            <details>
                <summary>
                    設定値
                </summary>
                <div>
                    <img width="300" src="https://user-images.githubusercontent.com/33025461/128602706-94a9c7cb-69c2-4f7d-ab19-025b85388fc0.png">
                </div>
            </details>
        </td>
        <td>
            <details>
                <summary>
                    設定値
                </summary>
                <div>
                    <img width="300" src="https://user-images.githubusercontent.com/33025461/128602916-f45fd0df-5e2f-43e5-a37d-e74b37195368.png">
                </div>
            </details>
        </td>
        <td>
            <details>
                <summary>
                    設定値
                </summary>
                <div>
                    <img width="300" src="https://user-images.githubusercontent.com/33025461/128603013-2fa7c77e-3b1a-4245-9a7c-2dcdfe02f69b.png">
                </div>
            </details>
        </td>
        <td>
            <details>
                <summary>
                    設定値
                </summary>
                <div>
                    <img width="300" src="https://user-images.githubusercontent.com/33025461/128603080-6b1340ab-6134-4658-9141-e61a66b100a3.png">
                </div>
            </details>
        </td>
        <td>
            <details>
                <summary>
                    設定値
                </summary>
                <div>
                    <img width="300" src="https://user-images.githubusercontent.com/33025461/128603154-57dd769f-ef09-471a-9556-2abb0f400a05.png">
                </div>
            </details>
        </td>
        <td>
            <details>
                <summary>
                    設定値
                </summary>
                <div>
                    <img width="300" src="https://user-images.githubusercontent.com/33025461/128637153-59340eaa-b65b-44d4-bca0-280c68d6a7e1.png">
                </div>
            </details>
        </td>
    </tr>
</table>

## How to

1. 色モードを設定
    `RGB, HSV, ...` から選択

1. 画像サイズの設定

    <img width="407" alt="スクリーンショット 2021-08-07 22 46 34" src="https://user-images.githubusercontent.com/33025461/128602312-77e834d6-c85c-4932-8a37-449903a7f023.png">

1. 1軸ごと設定(図はRGBモード)
    1. X, Y軸の適応比率を設定

        <img width="367" alt="スクリーンショット 2021-08-07 22 38 07" src="https://user-images.githubusercontent.com/33025461/128602085-3d4e9820-0383-4a69-9781-6938cef55ead.png">

    1. 座標に対する値をX, Y軸ごとに設定

        <img width="369" alt="スクリーンショット 2021-08-07 22 40 03" src="https://user-images.githubusercontent.com/33025461/128602136-1e886f37-fe7a-4b72-ba13-987a2562a8cc.png">

    1. 通常の直行座標ではなく特殊な座標を利用する場合(プレビュー画像の赤がX軸の値で緑がY軸の値)

        (この設定は5x5タイルでそれぞれで円形)

        <img width="403" alt="スクリーンショット 2021-08-07 22 42 27" src="https://user-images.githubusercontent.com/33025461/128602190-f5184406-ec14-4e28-b2c5-73089f24cf18.png">

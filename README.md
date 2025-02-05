# ColumnExplorer

 A windows application that allows you to navigate directories using arrow keys.

 Left column: Parent directory

 Center column: Current directory

 Right column: Preview selected file or folder

:)
--
 
**Move dir, Copy, Paste, Delete, Foward, Backward**

https://github.com/user-attachments/assets/56be2c53-585b-4bbf-b57e-624226f39991

;)
--
 
**Move file, Undo(ctrl+Z), Redo(ctrl+Y)**

https://github.com/user-attachments/assets/25111e66-c059-483f-84f7-d72799424df4

XD
--
 
**Preview Text, Pictures, PDF, Word, PowerPoint file**

![image](https://github.com/user-attachments/assets/108236a4-6ff6-49a9-bdfb-1277ffe9621b)

 ^^) _旦~~
--
 

※日本語の取説はページ後半にございます

# ColumnExplorer User Guide

## Overview
ColumnExplorer is a WPF application designed to help users navigate and manage their file system efficiently. It provides a multi-column interface for browsing directories and files, with features such as drag-and-drop, undo/redo actions, and file previews.

## Features
- **Multi-Column Navigation**: Browse directories and files in a multi-column layout.
- **Drag-and-Drop**: Easily move files and directories between columns.
- **Undo/Redo**: Undo and redo actions for file operations.
- **File Previews**: Preview text files, images, PDFs, and more.
- **Keyboard Shortcuts**: Navigate and manage files using keyboard shortcuts.

## Getting Started
1. **Launch the Application**: Open the ColumnExplorer application.
2. **Home Directory**: The application starts in the user's home directory.
3. **Column Layout**: The interface consists of three columns - Left, Center, and Right.

## Navigation
- **Mouse Navigation**:
  - Click on items in any column to navigate into directories.
  - Click on the column labels to reload the content of that column.
  - Use the mouse wheel to scroll through items.

- **Keyboard Shortcuts**:
  - `Right Arrow (→)`: Move to a subdirectory.
  - `Left Arrow (←)`: Move to the parent directory.
  - `Up Arrow (↑)`: Select the previous item.
  - `Down Arrow (↓)`: Select the next item.
  - `Enter`: Open selected items.
  - `F2`: Rename the selected item。
  - `F5`: Refresh the current directory.
  - `Ctrl + A`: Select all items.
  - `Ctrl + W`: Close the application.
  - `Ctrl + C`: Copy selected items to clipboard.
  - `Ctrl + V`: Paste items from clipboard.
  - `Ctrl + X`: Cut selected items.
  - `Delete`: Delete selected items.
  - `Ctrl + Shift + N`: Create a new folder.
  - `Ctrl + Z`: Undo the last action.
  - `Ctrl + Y`: Redo the last undone action.

## Drag-and-Drop
- **Start Dragging**: Click and hold on an item, then move the mouse to start dragging.
- **Drop Items**: Release the mouse button to drop items into a different column or directory.

## File Previews
- **Text Files**: Preview `.txt` files directly in the right column.
- **Word Documents**: Preview `.docx` files.
- **PowerPoint Presentations**: Preview `.pptx` files.
- **Images**: Preview common image formats such as `.jpg`, `.png`, `.bmp`, etc.
- **PDF Files**: Preview `.pdf` files.
- **Unsupported Files**: Display properties of unsupported file types.

## Undo/Redo
- **Undo**: Revert the last action using `Ctrl + Z`.
- **Redo**: Reapply the last undone action using `Ctrl + Y`.

## Creating New Folders
- **New Folder**: Create a new folder in the current directory using `Ctrl + Shift + N`.

## Deleting Items
- **Delete**: Remove selected items using the `Delete` key.

## Closing the Application
- **Close**: Exit the application using `Ctrl + W`.

## Tips
- **Focus Management**: Use the mouse or keyboard to focus on different columns for specific actions.
- **Temporary Feedback**: Actions like undo and redo provide temporary feedback in the column labels.

Enjoy using ColumnExplorer to manage your files efficiently!


# ColumnExplorer ユーザーガイド

## 概要
ColumnExplorer は、ファイルシステムを効率的にナビゲートおよび管理するための WPF アプリケーションです。複数のカラムインターフェースを提供し、ドラッグ＆ドロップ、元に戻す/やり直し、ファイルプレビューなどの機能を備えています。

## 機能
- **マルチカラムナビゲーション**: 複数のカラムレイアウトでディレクトリとファイルをブラウズ。
- **ドラッグ＆ドロップ**: ファイルやディレクトリを簡単にカラム間で移動。
- **元に戻す/やり直し**: ファイル操作の元に戻すとやり直し。
- **ファイルプレビュー**: テキストファイル、画像、PDF などのプレビュー。
- **キーボードショートカット**: キーボードショートカットを使用してファイルをナビゲートおよび管理。

## はじめに
1. **アプリケーションの起動**: ColumnExplorer アプリケーションを開きます。
2. **ホームディレクトリ**: アプリケーションはユーザーのホームディレクトリから開始します。
3. **カラムレイアウト**: インターフェースは左、中央、右の3つのカラムで構成されています。

## ナビゲーション
- **マウスナビゲーション**:
  - 任意のカラム内のアイテムをクリックしてディレクトリに移動。
  - カラムラベルをクリックしてそのカラムの内容を再読み込み。
  - マウスホイールを使用してアイテムをスクロール。

- **キーボードショートカット**:
  - `右矢印 (→)`: サブディレクトリに移動。
  - `左矢印 (←)`: 親ディレクトリに移動。
  - `上矢印 (↑)`: 前のアイテムを選択。
  - `下矢印 (↓)`: 次のアイテムを選択。
  - `Enter`: 選択したアイテムを開く。
  - `F2`: 選択したアイテムの名前を変更。
  - `F5`: 現在のディレクトリを更新。
  - `Ctrl + A`: すべてのアイテムを選択。
  - `Ctrl + W`: アプリケーションを閉じる。
  - `Ctrl + C`: 選択したアイテムをクリップボードにコピー。
  - `Ctrl + V`: クリップボードからアイテムを貼り付け。
  - `Ctrl + X`: 選択したアイテムをカット。
  - `Delete`: 選択したアイテムを削除。
  - `Ctrl + Shift + N`: 新しいフォルダーを作成。
  - `Ctrl + Z`: 最後の操作を元に戻す。
  - `Ctrl + Y`: 最後に元に戻した操作をやり直す。

## ドラッグ＆ドロップ
- **ドラッグ開始**: アイテムをクリックして保持し、マウスを動かしてドラッグを開始。
- **アイテムのドロップ**: マウスボタンを離してアイテムを別のカラムやディレクトリにドロップ。

## ファイルプレビュー
- **テキストファイル**: `.txt` ファイルを右カラムでプレビュー。
- **Word ドキュメント**: `.docx` ファイルをプレビュー。
- **PowerPoint プレゼンテーション**: `.pptx` ファイルをプレビュー。
- **画像**: `.jpg`, `.png`, `.bmp` などの一般的な画像形式をプレビュー。
- **PDF ファイル**: `.pdf` ファイルをプレビュー。
- **サポートされていないファイル**: サポートされていないファイル形式のプロパティを表示。

## 元に戻す/やり直し
- **元に戻す**: `Ctrl + Z` を使用して最後の操作を元に戻す。
- **やり直し**: `Ctrl + Y` を使用して最後に元に戻した操作をやり直す。

## 新しいフォルダーの作成
- **新しいフォルダー**: `Ctrl + Shift + N` を使用して現在のディレクトリに新しいフォルダーを作成。

## アイテムの削除
- **削除**: `Delete` キーを使用して選択したアイテムを削除。

## アプリケーションの終了
- **終了**: `Ctrl + W` を使用してアプリケーションを終了。

## ヒント
- **フォーカス管理**: マウスやキーボードを使用して特定のアクションのために異なるカラムにフォーカス。
- **一時的なフィードバック**: 元に戻すややり直しなどのアクションはカラムラベルに一時的なフィードバックを提供。

ColumnExplorer を使用してファイルを効率的に管理しましょう！

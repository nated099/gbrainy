copy ..\data\*.svg .
gmcs -target:winexe -out:gbrainy.exe SVGImage.cs PuzzleGames/PuzzleFourSided.cs Preferences.cs GtkDialog.cs PlayerHistory.cs PlayerHistoryDialog.cs PuzzleGames/PuzzleSquaresAndLetters.cs PuzzleGames/PuzzleTrianglesWithNumbers.cs CalculationGames/CalculationFractions.cs PuzzleGames/PuzzleCountCircles.cs PuzzleGames/PuzzleEquation.cs PuzzleGames/PuzzleQuadrilaterals.cs PuzzleGames/PuzzleCountSeries.cs PuzzleGames/PuzzleExtraCircle.cs  ArrayListIndicesRandom.cs GameManager.cs PuzzleGames/PuzzleCirclesRectangle.cs Game.cs PuzzleGames/PuzzleFigures.cs PuzzleGames/PuzzleMatrixNumbers.cs PuzzleGames/PuzzleMoveFigure.cs PuzzleGames/PuzzlePencil.cs PuzzleGames/PuzzleSquares.cs PuzzleGames/PuzzleTriangles.cs PuzzleGames/PuzzleCoverPercentage.cs PuzzleGames/PuzzleNumericSequence.cs PuzzleGames/PuzzleSquareDots.cs PuzzleGames/PuzzleNumericRelation.cs PuzzleGames/PuzzleNextFigure.cs PuzzleGames/PuzzleSquareSheets.cs CalculationGames/CalculationArithmetical.cs MemoryGames/MemoryColouredFigures.cs GameSession.cs MemoryGames/MemoryNumbers.cs Memory.cs MemoryGames/MemoryColouredText.cs PuzzleGames/PuzzleCube.cs MemoryGames/MemoryWords.cs PuzzleGames/PuzzleFigureLetter.cs CustomGameDialog.cs PuzzleGames/PuzzleDivideCircle.cs CalculationGames/CalculationGreatestDivisor.cs CalculationGames/CalculationTwoNumbers.cs CalculationGames/CalculationWhichNumber.cs PuzzleGames/PuzzleMatrixGroups.cs PuzzleGames/PuzzleBalance.cs PuzzleGames/PuzzleOstracism.cs MemoryGames/MemoryCountDots.cs CalculationGames/CalculationOperator.cs PuzzleGames/PuzzleFigurePattern.cs ColorPalette.cs PuzzleGames/PuzzlePeopleTable.cs GameDrawingArea.cs MemoryGames/MemoryFigures.cs PuzzleGames/PuzzleMissingSlice.cs PuzzleGames/PuzzleLines.cs PuzzleGames/PuzzleTetris.cs PreferencesDialog.cs PuzzleGames/PuzzleMissingPiece.cs MemoryGames/MemoryIndications.cs PuzzleGames/PuzzleMostInCommon.cs PuzzleGames/PuzzleBuildTriangle.cs CairoContextEx.cs PuzzleGames/PuzzleClocks.cs PuzzleGames/PuzzleLargerShape.cs MemoryGames/MemoryFiguresNumbers.cs gbrainy.cs Defines.cs -pkg:gnome-sharp-2.0 -pkg:gtk-sharp-2.0 -pkg:glade-sharp-2.0 -r:Mono.Cairo.dll -r:Mono.Posix -resource:gbrainy.glade -resource:../data/resume-32.png -resource:../data/endgame-32.png -resource:../data/pause-32.png -resource:../data/allgames-32.png -resource:../data/gbrainy.png -resource:../data/logic-games-32.png -resource:../data/math-games-32.png -resource:../data/memory-games-32.png -resource:../data/gbrainy.svg




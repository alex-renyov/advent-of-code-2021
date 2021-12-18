package ru.renev.adventofcode2021

class Day4 {
    fun part1(): Int {
        println("Day 4 part 1")

        val numbers = parseNumbers()
        val (boards, size) = parseBoards()

        val drawn = HashSet<Int>()

        for (number in numbers) {
            drawn.add(number)
            val winning = boards.find { board -> isWinning(board, size, drawn) }
            if (winning == null) {
                continue
            }

            val unmarkedSum = getValues(winning, size)
                .filter { !drawn.contains(it) }
                .sum()
            return unmarkedSum * number
        }

        return 0
    }

    fun part2(): Int {
        println("Day 4 part 2")

        val numbers = parseNumbers()
        val (boards, size) = parseBoards()

        val fullDrawn = numbers.toHashSet()
        val winningBoards = boards.filter { board -> isWinning(board, size, fullDrawn) }.toList()

        val drawn = HashSet<Int>()
        var remainingBoards = winningBoards

        for (number in numbers) {
            drawn.add(number)

            val winning = remainingBoards.filter { board -> isWinning(board, size, drawn) }.toList()
            if (winning.isEmpty()) {
                continue
            }

            remainingBoards = remainingBoards.minus(winning)
            if (remainingBoards.isEmpty()) {
                val unmarkedSum = getValues(winning.first(), size).filter { !drawn.contains(it) }.sum()
                return unmarkedSum * number
            }
        }

        return 0
    }

    private fun isWinning(board: List<List<Int>>, size: Int, drawn: Set<Int>): Boolean {
        return makeLines(board, size).any { line ->
            line.all { drawn.contains(it) }
        }
    }

    private fun getValues(board: List<List<Int>>, size: Int): Sequence<Int> {
        return sequence<Int> {
            (0 until size).forEach { x ->
                (0 until size).forEach { y ->
                    yield(board[x][y])
                }
            }
        }
    }

    private fun makeLines(board: List<List<Int>>, size: Int): Sequence<List<Int>> {
        return sequence<List<Int>> {
            (0 until size).forEach {
                yield((0 until size).map { y -> board[it][y] })
                yield((0 until size).map { y -> board[y][it] })
            }
        }
    }

    private fun parseNumbers(): List<Int> {
        return Day4Input.numbers.split(',').map { it.toInt() }
    }

    private fun parseBoards(): Pair<List<List<List<Int>>>, Int> {
        val boards = Day4Input.boards.trim().split("\n\n")
            .map { group ->
                group.trim()
                    .split("\n")
                    .filter { it.isNotEmpty() }
                    .map { line ->
                        line.trim()
                            .split(' ')
                            .filter { it.isNotEmpty() }
                            .map { it.toInt() }
                    }
            }

        val size = boards.first().count()

        return Pair(boards, size)
    }
}
